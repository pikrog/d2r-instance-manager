using AvaloniaApplication1.Design.Services;
using AvaloniaApplication1.GlobalSettings;

namespace AvaloniaApplication1.Design.ViewModels;

public class GlobalSettingsPageDesignViewModel : GlobalSettingsPageViewModel
{
    public GlobalSettingsPageDesignViewModel() : base(DesignServices.GlobalSettingsService,
        DesignServices.GameExecutablePathLocator,
        new FakeGlobalSettingsPathValidator(),
        DesignServices.ToastService,
        DesignServices.OverlayService)
    {
        ValidateGameExecutablePath();
    }
}

