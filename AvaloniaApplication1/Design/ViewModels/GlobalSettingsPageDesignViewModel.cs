using AvaloniaApplication1.Design.Services;
using AvaloniaApplication1.GlobalSettings;

namespace AvaloniaApplication1.Design.ViewModels;

public class GlobalSettingsPageDesignViewModel() : GlobalSettingsPageViewModel(DesignServices.GlobalSettingsService,
    DesignServices.GameExecutablePathLocator);