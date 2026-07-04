using System.Threading.Tasks;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Services;
using AvaloniaApplication1.ViewModels.Dialog;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.ViewModels;

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
    public partial bool CenterMouseCursorInRecalledWindow { get; set; }
    
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
        CenterMouseCursorInRecalledWindow = settings.CenterMouseCursorInRecalledWindow;
        CloseInstancesOnAppExit = settings.CloseInstancesOnAppExit;
    }

    public async Task SaveAsync()
    {
        var draft = new GlobalSettingsDraft(
            GameExecutablePath,
            CenterMouseCursorInRecalledWindow,
            FallbackToPrimaryDisplayIfInvalid,
            CloseInstancesOnAppExit
        );
        
        await _globalSettingsService.SaveAsync(draft);
    }

    [RelayCommand]
    public async Task BrowseForGameExecutablePath()
    {
        var path = await this.OpenFilePickerForGameExecutable();
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

    public override void OnEnter() => Refresh();

    public override bool OnLeave() => true;
}