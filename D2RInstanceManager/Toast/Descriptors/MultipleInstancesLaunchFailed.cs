namespace D2RInstanceManager.Toast.Descriptors;

public sealed record MultipleInstancesLaunchFailed(int Count) : ToastDescriptor(ToastSeverity.Error);