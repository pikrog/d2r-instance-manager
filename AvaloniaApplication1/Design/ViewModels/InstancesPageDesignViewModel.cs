using System;
using System.Threading.Tasks;
using AvaloniaApplication1.Design.Services;
using AvaloniaApplication1.Display;
using AvaloniaApplication1.GlobalSettings.Issues;
using AvaloniaApplication1.Instance.Models;
using AvaloniaApplication1.Instance.Models.Issues;
using AvaloniaApplication1.Instance.ViewModels;
using DynamicData;

namespace AvaloniaApplication1.Design.ViewModels;

public class InstancesPageDesignViewModel : InstancesPageViewModel
{
    public InstancesPageDesignViewModel() : base(DesignServices.InstanceService,
        DesignServices.AccountService,
        DesignServices.RegionService,
        DesignServices.DisplayService,
        DesignServices.GlobalSettingsValidator,
        DesignServices.OverlayService)
    {
        FirstGlobalSettingsIssue = new MissingExecutablePath();
        SetupDesignPage();
    }

    private void SetupDesignPage()
    {
        Instances.Clear();
        Instances.AddRange([
            new InstanceItemViewModel(Guid.NewGuid(), "Main", InstanceStatus.Running, true, []),
            new InstanceItemViewModel(Guid.NewGuid(), "Battle Orders", InstanceStatus.Starting, true, []),
            new InstanceItemViewModel(Guid.NewGuid(), "Enchant", InstanceStatus.Failed, false, []),
            new InstanceItemViewModel(Guid.NewGuid(), "Mule [EU]", InstanceStatus.Queued, true, []),
            new InstanceItemViewModel(Guid.NewGuid(), "Mule [US]", InstanceStatus.Inactive, false, [new MissingDisplay(new CachedDisplaySnapshot("fake", "LG", 1440, 800), true)]),
            new InstanceItemViewModel(Guid.NewGuid(), "Mule [Asia]", InstanceStatus.Inactive, false, [new DeletedAccount()]),
        ]);
        
        /*GlobalSettingsIssues.Clear();
        GlobalSettingsIssues.Add(new MissingExecutablePath());*/
    }

    public override Task OnEnterAsync()
    {
        SetupDesignPage();
        return Task.CompletedTask;
    }
}
