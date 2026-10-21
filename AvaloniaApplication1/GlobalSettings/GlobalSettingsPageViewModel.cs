using System.Threading.Tasks;
using AvaloniaApplication1.Dialog;
using AvaloniaApplication1.GameExecutable;
using AvaloniaApplication1.Navigation;
using AvaloniaApplication1.Toast;
using AvaloniaApplication1.Toast.Descriptors;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.GlobalSettings;

public partial class GlobalSettingsPageViewModel : PageViewModel, IDialogParticipant
{
    private readonly GlobalSettingsService _globalSettingsService;
    
    private readonly GameExecutablePathLocator _gameExecutablePathLocator;
    
    private readonly ToastService _toastService;

    [ObservableProperty]
    public partial string GameExecutablePath { get; set; } = "";
    
    [ObservableProperty]
    public partial bool FallbackToPrimaryDisplayIfInvalid { get; set; }
    
    [ObservableProperty]
    public partial bool CenterMouseCursorInShownWindow { get; set; }
    
    [ObservableProperty]
    public partial bool CloseInstancesOnAppExit { get; set; }
    
    private GlobalSettingsDraft? _savedDraft;
    
    public GlobalSettingsPageViewModel(GlobalSettingsService globalSettingsService,
        GameExecutablePathLocator gameExecutablePathLocator,
        ToastService toastService)
    {
        _globalSettingsService = globalSettingsService;
        _gameExecutablePathLocator = gameExecutablePathLocator;
        _toastService = toastService;
    }

    public void Refresh()
    {
        var settings = _globalSettingsService.GetSnapshot(); // todo: Get Snapshot Projection for UI?
        
        GameExecutablePath = settings.GameExecutablePath;
        FallbackToPrimaryDisplayIfInvalid = settings.FallbackToPrimaryDisplayIfInvalid;
        CenterMouseCursorInShownWindow = settings.CenterMouseCursorInShownWindow;
        CloseInstancesOnAppExit = settings.CloseInstancesOnAppExit;
        
        _savedDraft = CreateDraft();
    }

    private GlobalSettingsDraft CreateDraft() => new(GameExecutablePath, 
        CenterMouseCursorInShownWindow,
        FallbackToPrimaryDisplayIfInvalid,
        CloseInstancesOnAppExit);

    public async Task SaveAsync()
    {
        var draft = CreateDraft();
        if (_savedDraft is { } savedDraft && draft == savedDraft)
            return;
        
        await _globalSettingsService.SaveAsync(draft);
        
        _savedDraft = draft;
    }

    [RelayCommand]
    public async Task BrowseForGameExecutablePath()
    {
        var openFileOptions = GameExecutablePickerOptionsFactory.Create();
        var path = await this.PickPathByOpenFilePicker(openFileOptions);
        if (path is not null)
            GameExecutablePath = path;
    }

    [RelayCommand]
    public void AutoDetectGameExecutablePath()
    {
        var path = _gameExecutablePathLocator.TryLocate();
        
        if (path is null)
        {
            var errorToast = new GameExecutableNotFound();
            _toastService.Show(errorToast);
            return;
        }
        
        GameExecutablePath = path;
        
        var toast = new GameExecutableFound(path);
        _toastService.Show(toast);
    }

    public override Task OnEnterAsync()
    {
        Refresh();
        return Task.CompletedTask;
    }
    
    public override async Task OnLeaveAsync() => await SaveAsync();
}