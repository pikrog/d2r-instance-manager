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
    public InstancesPageDesignViewModel() : base(DesignServices.GameInstanceService,
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
            new GameInstanceItemViewModel(Guid.NewGuid(), "Main", GameInstanceStatus.Running, true, []),
            new GameInstanceItemViewModel(Guid.NewGuid(), "Battle Orders", GameInstanceStatus.Starting, true, []),
            new GameInstanceItemViewModel(Guid.NewGuid(), "Enchant", GameInstanceStatus.Failed, false, []),
            new GameInstanceItemViewModel(Guid.NewGuid(), "Mule [EU]", GameInstanceStatus.Queued, true, []),
            new GameInstanceItemViewModel(Guid.NewGuid(), "Mule [US]", GameInstanceStatus.Inactive, false, [new MissingDisplay(new CachedDisplaySnapshot("fake", "LG", 1440, 800), true)]),
            new GameInstanceItemViewModel(Guid.NewGuid(), "Mule [Asia]", GameInstanceStatus.Inactive, false, [new DeletedAccount()]),
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
