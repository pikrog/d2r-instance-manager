using System.Threading.Tasks;
using AvaloniaApplication1.Dialog;
using AvaloniaApplication1.GameExecutable;
using AvaloniaApplication1.GlobalSettings.Issues;
using AvaloniaApplication1.Navigation;
using AvaloniaApplication1.Overlay;
using AvaloniaApplication1.Overlay.Dialog.SetPassphrase;
using AvaloniaApplication1.Semantic;
using AvaloniaApplication1.Toast;
using AvaloniaApplication1.Toast.Descriptors;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.GlobalSettings;

public partial class GlobalSettingsPageViewModel(
    GlobalSettingsService globalSettingsService,
    IGameExecutablePathLocator gameExecutablePathLocator,
    IGlobalSettingsPathValidator gameExecutablePathValidator,
    ToastService toastService,
    OverlayService overlayService)
    : PageViewModel, IDialogParticipant
{
    [ObservableProperty]
    public partial string GameExecutablePath { get; set; } = "";
    
    [ObservableProperty]
    public partial bool FallbackToPrimaryDisplayIfInvalid { get; set; }
    
    [ObservableProperty]
    public partial bool CenterMouseCursorInShownWindow { get; set; }
    
    [ObservableProperty]
    public partial bool CloseInstancesOnAppExit { get; set; }

    private string? _passphrase;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsProtectionDisabledSelected),
        nameof(IsProtectionByWindowsAccountSelected),
        nameof(IsProtectionByPassphraseSelected))]
    public partial ProtectionSelection SelectedProtection { get; set; }
    
    public bool IsProtectionDisabledSelected => SelectedProtection == ProtectionSelection.Disabled;
    
    public bool IsProtectionByWindowsAccountSelected => SelectedProtection == ProtectionSelection.ProtectedByWindowsAccount;
    
    public bool IsProtectionByPassphraseSelected => SelectedProtection == ProtectionSelection.ProtectedByPassphrase;

    private GlobalSettingsDraft? _savedDraft;

    private ProtectionSelection? _savedProtectionSelection;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(GameExecutablePathIssueSemantic), nameof(IsGameExecutablePathInvalid))]
    public partial GameExecutablePathIssue? GameExecutablePathIssue { get; private set; }
    
    public SemanticType? GameExecutablePathIssueSemantic
    {
        get
        {
            if (GameExecutablePathIssue is null)
                return null;
            
            return GameExecutablePathIssue.RequiresAttention
                ? SemanticType.Danger
                : SemanticType.Warning;
        }
    }
    
    public bool IsGameExecutablePathInvalid => 
        GameExecutablePathIssue is not null && GameExecutablePathIssue.RequiresAttention;

    private void Refresh()
    {
        var settings = globalSettingsService.GetSnapshot();
        
        GameExecutablePath = settings.GameExecutablePath;
        FallbackToPrimaryDisplayIfInvalid = settings.FallbackToPrimaryDisplayIfInvalid;
        CenterMouseCursorInShownWindow = settings.CenterMouseCursorInShownWindow;
        CloseInstancesOnAppExit = settings.CloseInstancesOnAppExit;
        SelectedProtection = ProtectionSelectionMapper.Map(settings.SecretProtection.EncryptionOptions);
        
        _savedDraft = CreateDraft();
        _savedProtectionSelection = SelectedProtection;
    }

    private GlobalSettingsDraft CreateDraft() => new(GameExecutablePath, 
        CenterMouseCursorInShownWindow,
        FallbackToPrimaryDisplayIfInvalid,
        CloseInstancesOnAppExit);

    private async Task SaveDraftIfChangedAsync()
    {
        var draft = CreateDraft();
        if (_savedDraft is { } savedDraft && draft == savedDraft)
            return;
        
        await globalSettingsService.SaveAsync(draft);
        
        _savedDraft = draft;
    }

    private async Task SetProtectionIfChangedAsync()
    {
        if (SelectedProtection == _savedProtectionSelection || 
            (SelectedProtection == ProtectionSelection.ProtectedByPassphrase && _passphrase is null))
            return;
        
        await globalSettingsService.SetProtectionSettingsAsync(SelectedProtection, _passphrase);
        
        _savedProtectionSelection = SelectedProtection;
    }

    private async Task SaveAsync()
    {
        await SaveDraftIfChangedAsync();
        await SetProtectionIfChangedAsync();
    }

    [RelayCommand]
    private async Task BrowseForGameExecutablePath()
    {
        var openFileOptions = GameExecutablePickerOptionsFactory.Create();
        var path = await this.PickPathByOpenFilePicker(openFileOptions);
        if (path is not null)
            GameExecutablePath = path;
        
        ValidateGameExecutablePath();
    }

    [RelayCommand]
    private void AutoDetectGameExecutablePath()
    {
        var path = gameExecutablePathLocator.TryLocate();
        
        if (path is null)
        {
            var errorToast = new GameExecutableNotFound();
            toastService.Show(errorToast);
            return;
        }
        
        GameExecutablePath = path;
        
        ValidateGameExecutablePath();
        
        var toast = new GameExecutableFound(path);
        toastService.Show(toast);
    }

    [RelayCommand]
    private async Task SelectProtectionOption(ProtectionSelection selection)
    {
        SelectedProtection = selection;
        
        if (selection == ProtectionSelection.ProtectedByPassphrase 
            && _savedProtectionSelection != ProtectionSelection.ProtectedByPassphrase 
            && _passphrase is null)
        {
            var isSet = await SetPassphrase();
            if (!isSet)
                SelectedProtection = _savedProtectionSelection ?? ProtectionSelection.Disabled;
        }
    }

    [RelayCommand]
    private async Task<bool> SetPassphrase()
    {
        var dialog = new SetPassphraseDialogViewModel();
        var result = await overlayService.ShowAsync(dialog);

        if (result is not SetPassphraseResult.Entered { Passphrase: var passphrase })
            return false;
        
        _passphrase = passphrase;
        return true;

    }

    public void ValidateGameExecutablePath() => 
        GameExecutablePathIssue = gameExecutablePathValidator.Validate(GameExecutablePath);

    public override Task OnEnterAsync()
    {
        Refresh();
        ValidateGameExecutablePath();
        return Task.CompletedTask;
    }
    
    public override async Task OnLeaveAsync() => await SaveAsync();
}