using AvaloniaApplication1.Design.Services;
using AvaloniaApplication1.ViewModels;
using GlobalSettingsPageViewModel = AvaloniaApplication1.ViewModels.GlobalSettings.GlobalSettingsPageViewModel;

namespace AvaloniaApplication1.Design.ViewModels;

public class GlobalSettingsPageDesignViewModel() : AvaloniaApplication1.ViewModels.GlobalSettings.GlobalSettingsPageViewModel(DesignServices.GlobalSettingsService,
    DesignServices.GameExecutablePathLocator);