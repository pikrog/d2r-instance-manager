using System.Threading.Tasks;
using AvaloniaApplication1.Dialog;
using AvaloniaApplication1.GameExecutable;
using AvaloniaApplication1.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.GlobalSettings;

public partial class GlobalSettingsPageViewModel : PageViewModel, IDialogParticipant
{
    private readonly GlobalSettingsService _globalSettingsService;
    
    private readonly GameExecutablePathLocator _gameExecutablePathLocator;

    [ObservableProperty]
    // [Required]
    public partial string GameExecutablePath { get; set; } = "";
    
    [ObservableProperty]
    public partial bool FallbackToPrimaryDisplayIfInvalid { get; set; }
    
    [ObservableProperty]
    public partial bool CenterMouseCursorInShownWindow { get; set; }
    
    [ObservableProperty]
    public partial bool CloseInstancesOnAppExit { get; set; }
    
    public GlobalSettingsPageViewModel(GlobalSettingsService globalSettingsService,
        GameExecutablePathLocator gameExecutablePathLocator)
    {
        _globalSettingsService = globalSettingsService;
        _gameExecutablePathLocator = gameExecutablePathLocator;
        
        Refresh();
    }

    public void Refresh()
    {
        var settings = _globalSettingsService.GetSnapshot(); // todo: Get Snapshot Projection for UI?
        GameExecutablePath = settings.GameExecutablePath;
        FallbackToPrimaryDisplayIfInvalid = settings.FallbackToPrimaryDisplayIfInvalid;
        CenterMouseCursorInShownWindow = settings.CenterMouseCursorInShownWindow;
        CloseInstancesOnAppExit = settings.CloseInstancesOnAppExit;
    }

    public async Task SaveAsync()
    {
        var draft = new GlobalSettingsDraft(
            GameExecutablePath,
            CenterMouseCursorInShownWindow,
            FallbackToPrimaryDisplayIfInvalid,
            CloseInstancesOnAppExit
        );
        
        await _globalSettingsService.SaveAsync(draft);
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
        if (path is not null)
            GameExecutablePath = path;
    }

    public override Task OnEnterAsync()
    {
        Refresh();
        return Task.CompletedTask;
    }
    
    public override async Task OnLeaveAsync() => await SaveAsync(); // todo: publish save request to some serialization service
}