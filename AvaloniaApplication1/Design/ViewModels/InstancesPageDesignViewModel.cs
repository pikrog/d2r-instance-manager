using System;
using AvaloniaApplication1.Design.Services;
using AvaloniaApplication1.Engine.Models;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Services;
using AvaloniaApplication1.ViewModels;
using DynamicData;
using InstancesPageViewModel = AvaloniaApplication1.ViewModels.Instance.InstancesPageViewModel;

namespace AvaloniaApplication1.Design.ViewModels;

public class InstancesPageDesignViewModel : AvaloniaApplication1.ViewModels.Instance.InstancesPageViewModel
{
    public InstancesPageDesignViewModel() : base(DesignServices.GameInstanceService,
        DesignServices.AccountService,
        DesignServices.RegionService,
        DesignServices.DisplayService)
    {
        Instances.Clear();
        
        Instances.AddRange([
            new GameInstanceTableRow(Guid.NewGuid(), "Main", GameInstanceStatus.Running, true),
            new GameInstanceTableRow(Guid.NewGuid(), "Battle Orders", GameInstanceStatus.Starting, true),
            new GameInstanceTableRow(Guid.NewGuid(), "Enchant", GameInstanceStatus.Failed, false),
            new GameInstanceTableRow(Guid.NewGuid(), "Mule [EU]", GameInstanceStatus.QueuedForStart, false),
            new GameInstanceTableRow(Guid.NewGuid(), "Mule [US]", GameInstanceStatus.Inactive, false),
        ]);
    }
}
