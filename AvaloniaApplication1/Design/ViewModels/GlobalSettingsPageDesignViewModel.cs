using AvaloniaApplication1.Design.Services;
using AvaloniaApplication1.ViewModels;

namespace AvaloniaApplication1.Design.ViewModels;

public class GlobalSettingsPageDesignViewModel() : GlobalSettingsPageViewModel(DesignServices.GlobalSettingsService);