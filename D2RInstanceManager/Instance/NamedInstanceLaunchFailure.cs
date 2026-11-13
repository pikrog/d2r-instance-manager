using D2RInstanceManager.Instance.Models;

namespace D2RInstanceManager.Instance;

public sealed record NamedInstanceLaunchFailure(string Name, InstanceLaunchFailure Failure);