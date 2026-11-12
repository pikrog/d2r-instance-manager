using System.Collections.Generic;
using System.Linq;
using D2RInstanceManager.Engine.Common;
using D2RInstanceManager.Engine.Models.StateMachine;
using D2RInstanceManager.Log;
using D2RInstanceManager.Log.Instance;
using D2RInstanceManager.Notification;
using D2RInstanceManager.Toast;
using D2RInstanceManager.Toast.Descriptors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace D2RInstanceManager.Instance;

public class InstanceExitReporter(
    InstanceNameRegistry instanceNameRegistry,
    [FromKeyedServices(NotificationCounterKey.Instances)]
    INotificationSink notificationSink,
    ToastService toastService,
    ILogger<InstanceExitReporter> logger)
{
    private void LogExit(RuntimeSnapshot snapshot)
    {
        if (snapshot.Errors.Any())
        {
            logger.LogError("Instance failed");
            return;
        }
        
        switch (snapshot.ProcessExitResult)
        {
            case ProcessExitResult.Failure:
                logger.LogError("Instance crashed");
                break;
            case ProcessExitResult.Terminated:
                logger.LogWarning("Instance terminated");
                break;
            default:
                logger.LogInformation("Instance stopped");
                break;
        }
    }

    private void ShowExitToast(RuntimeSnapshot snapshot, string? instanceName)
    {
        if (instanceName is null)
            return;
        
        ToastDescriptor? toast = null;

        if (snapshot.Errors.Any())
            toast = new InstanceFailed(instanceName, snapshot.Errors[0]);
        else
            toast = snapshot.ProcessExitResult switch
            {
                ProcessExitResult.Failure => new InstanceCrashed(instanceName),
                ProcessExitResult.Terminated => new InstanceTerminated(instanceName),
                _ => toast
            };

        if (toast is not null)
            toastService.Show(toast);
    }
    
    public void Report(RuntimeSnapshot snapshot)
    {
        if (snapshot.Errors.Any()|| snapshot.ProcessExitResult is not ProcessExitResult.Success)
            notificationSink.Increment();

        var instanceName = instanceNameRegistry.Get(snapshot.Id);
        using var _ = logger.BeginScope(new Dictionary<string, object?> { { LogProperties.InstanceName, instanceName } });

        LogExit(snapshot);
        ShowExitToast(snapshot, instanceName);
    }
}