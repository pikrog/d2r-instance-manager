using D2RInstanceManager.Instance;

namespace D2RInstanceManager.Toast.Descriptors;

public sealed record InstanceLaunchFailed(NamedInstanceLaunchFailure Failure) : ToastDescriptor(ToastSeverity.Error);