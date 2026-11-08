using D2RInstanceManager.Design.Services;
using D2RInstanceManager.GlobalSettings;

namespace D2RInstanceManager.Design.ViewModels;

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

