using System;
using System.Collections.Generic;
using D2RInstanceManager.Config;
using D2RInstanceManager.Display;
using D2RInstanceManager.Engine;
using D2RInstanceManager.Instance;
using D2RInstanceManager.Instance.Models;
using D2RInstanceManager.Instance.Models.Issues;
using D2RInstanceManager.Log.Instance;
using D2RInstanceManager.Notification;
using Microsoft.Extensions.Logging;

namespace D2RInstanceManager.Design.Services;

public class InstanceDesignService(
    ConfigService configService,
    InstanceConfigValidator validator,
    InstanceManager manager,
    InstanceNameRegistry registry,
    InstanceLaunchContextFactory factory,
    InstanceActivityTracker activityTracker,
    INotificationSink notificationSink,
    ILogger<InstanceService> logger) 
    : InstanceService(configService, validator, manager, registry, factory, activityTracker, notificationSink, logger)
{
    public override IReadOnlyList<InstanceSummary> GetSummaries() => [
        new(Guid.NewGuid(), "Main", InstanceStatus.Running, true, []),
        new(Guid.NewGuid(), "Battle Orders", InstanceStatus.Starting, true, []),
        new(Guid.NewGuid(), "Enchant", InstanceStatus.Failed, false, []),
        new(Guid.NewGuid(), "Mule [EU]", InstanceStatus.Queued, true, []),
        new(Guid.NewGuid(), "Mule [US]", InstanceStatus.Inactive, false, [new MissingDisplay(new DisplaySnapshot("fake", "LG", 1440, 800), true)]),
        new(Guid.NewGuid(), "Mule [Asia]", InstanceStatus.Inactive, false, [new DeletedAccount()]),
    ];
}