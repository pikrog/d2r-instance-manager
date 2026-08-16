using System;
using System.Collections.Generic;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Display;
using AvaloniaApplication1.Engine;
using AvaloniaApplication1.Instance;
using AvaloniaApplication1.Instance.Models;
using AvaloniaApplication1.Instance.Models.Issues;

namespace AvaloniaApplication1.Design.Services;

public class InstanceDesignService(ConfigService configService, InstanceConfigValidator validator, InstanceManager manager) 
    : InstanceService(configService, validator, manager)
{
    public override IReadOnlyList<InstanceSummary> GetSummaries() => [
        new(Guid.NewGuid(), "Main", InstanceStatus.Running, true, []),
        new(Guid.NewGuid(), "Battle Orders", InstanceStatus.Starting, true, []),
        new(Guid.NewGuid(), "Enchant", InstanceStatus.Failed, false, []),
        new(Guid.NewGuid(), "Mule [EU]", InstanceStatus.Queued, true, []),
        new(Guid.NewGuid(), "Mule [US]", InstanceStatus.Inactive, false, [new MissingDisplay(new CachedDisplaySnapshot("fake", "LG", 1440, 800), true)]),
        new(Guid.NewGuid(), "Mule [Asia]", InstanceStatus.Inactive, false, [new DeletedAccount()]),
    ];
}