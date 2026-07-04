using AvaloniaApplication1.Design.Services;
using AvaloniaApplication1.ViewModels;
using GlobalSettingsPageViewModel = AvaloniaApplication1.ViewModels.GlobalSettingsPageViewModel;

namespace AvaloniaApplication1.Design.ViewModels;

public class GlobalSettingsPageDesignViewModel() : GlobalSettingsPageViewModel(DesignServices.GlobalSettingsService,
    DesignServices.GameExecutablePathLocator);