using System.Threading.Tasks;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.ViewModels;

public partial class GlobalSettingsPageViewModel : ViewModelBase
{
    private readonly GlobalSettingsService _globalSettingsService;

    [ObservableProperty]
    // [Required]
    public partial string GameExecutablePath { get; set; } = "";
    
    [ObservableProperty]
    public partial bool FallbackToPrimaryDisplayIfInvalid { get; set; }
    
    [ObservableProperty]
    public partial bool CenterMouseCursorInRecalledWindow { get; set; }
    
    [ObservableProperty]
    public partial bool CloseInstancesOnAppExit { get; set; }
    
    public GlobalSettingsPageViewModel(GlobalSettingsService globalSettingsService)
    {
        _globalSettingsService = globalSettingsService;
        
        Refresh();
    }

    public void Refresh()
    {
        var settings = _globalSettingsService.GetSnapshot(); // todo: Get Snapshot Projection for UI?
        GameExecutablePath = settings.GameExecutablePath;
        FallbackToPrimaryDisplayIfInvalid = settings.FallbackToPrimaryDisplayIfInvalid;
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
}