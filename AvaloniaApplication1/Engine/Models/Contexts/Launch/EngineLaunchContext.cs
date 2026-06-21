namespace AvaloniaApplication1.Engine.Models.Contexts.Launch;

public record EngineLaunchContext
(
    InstanceLaunchContext InstanceLaunchContext,
    EnginePolicies Policies
);