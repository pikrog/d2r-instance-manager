using System;
using AvaloniaApplication1.Design.Services;
using AvaloniaApplication1.Engine.Models;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Services;
using AvaloniaApplication1.ViewModels;
using DynamicData;

namespace AvaloniaApplication1.Design.ViewModels;

public class InstancesPageDesignViewModel : InstancesPageViewModel
{
    public InstancesPageDesignViewModel() : base(DesignServices.GameInstanceService, DesignServices.AccountService, DesignServices.RegionService) // todo: replace with DesignServices?
    {
        Instances.Clear();
        
        Instances.AddRange([
            new GameInstanceTableRow(Guid.NewGuid(), "Main", GameInstanceStatus.Running),
            new GameInstanceTableRow(Guid.NewGuid(), "Battle Orders", GameInstanceStatus.QueuedForStart),
            new GameInstanceTableRow(Guid.NewGuid(), "Enchant", GameInstanceStatus.Failed),
            new GameInstanceTableRow(Guid.NewGuid(), "Mule", GameInstanceStatus.Inactive),
        ]);
    }
}
