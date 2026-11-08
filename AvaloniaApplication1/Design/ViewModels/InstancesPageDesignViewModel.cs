using D2RInstanceManager.Design.Services;
using D2RInstanceManager.GlobalSettings.Issues;
using D2RInstanceManager.Instance.ViewModels;

namespace D2RInstanceManager.Design.ViewModels;

public sealed class InstancesPageDesignViewModel : InstancesPageViewModel
{
    public InstancesPageDesignViewModel() : base(DesignServices.InstanceService,
        DesignServices.InstancePresenter,
        DesignServices.GlobalSettingsValidator,
        DesignServices.OverlayService,
        DesignServices.NavigationService,
        DesignServices.EditInstanceFormViewModelFactory,
        DesignServices.InstanceNotificationCounter,
        DesignServices.DisplayConfigMonitor)
    {
        FirstGlobalSettingsIssue = new MissingExecutablePath();

        OnEnterAsync().GetAwaiter().GetResult();
    }
}
