using System;
using System.Threading.Tasks;
using AvaloniaApplication1.Design.Services;
using AvaloniaApplication1.Instance.Models;
using AvaloniaApplication1.Instance.ViewModels;
using DynamicData;
using GameInstanceItemViewModel = AvaloniaApplication1.Instance.ViewModels.GameInstanceItemViewModel;

namespace AvaloniaApplication1.Design.ViewModels;

public class InstancesPageDesignViewModel : InstancesPageViewModel
{
    public InstancesPageDesignViewModel() : base(DesignServices.GameInstanceService,
        DesignServices.AccountService,
        DesignServices.RegionService,
        DesignServices.DisplayService,
        DesignServices.OverlayService)
    {
        AddDesignInstances();
    }

    private void AddDesignInstances()
    {
        Instances.Clear();
        
        Instances.AddRange([
            new GameInstanceItemViewModel(Guid.NewGuid(), "Main", GameInstanceStatus.Running, true),
            new GameInstanceItemViewModel(Guid.NewGuid(), "Battle Orders", GameInstanceStatus.Starting, true),
            new GameInstanceItemViewModel(Guid.NewGuid(), "Enchant", GameInstanceStatus.Failed, false),
            new GameInstanceItemViewModel(Guid.NewGuid(), "Mule [EU]", GameInstanceStatus.QueuedForStart, false),
            new GameInstanceItemViewModel(Guid.NewGuid(), "Mule [US]", GameInstanceStatus.Inactive, false),
        ]);
    }

    public override Task OnEnterAsync()
    {
        AddDesignInstances();
        return Task.CompletedTask;
    }
}
