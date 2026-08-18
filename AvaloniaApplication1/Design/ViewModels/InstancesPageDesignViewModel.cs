using AvaloniaApplication1.Design.Services;
using AvaloniaApplication1.GlobalSettings.Issues;
using AvaloniaApplication1.Instance.ViewModels;

namespace AvaloniaApplication1.Design.ViewModels;

public sealed class InstancesPageDesignViewModel : InstancesPageViewModel
{
    public InstancesPageDesignViewModel() : base(DesignServices.InstanceService,
        DesignServices.AccountService,
        DesignServices.RegionService,
        DesignServices.DisplayService,
        DesignServices.GlobalSettingsValidator,
        DesignServices.OverlayService,
        DesignServices.NavigationService)
    {
        FirstGlobalSettingsIssue = new MissingExecutablePath();

        OnEnterAsync().GetAwaiter().GetResult();
    }
}
