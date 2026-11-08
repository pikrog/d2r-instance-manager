namespace D2RInstanceManager.Engine.Models.Contexts.Launch;

public record EngineLaunchContext
(
    InstanceLaunchContext InstanceLaunchContext,
    EnginePolicies Policies
);