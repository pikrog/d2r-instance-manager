namespace D2RInstanceManager.Toast.Descriptors;

public sealed record InstanceTerminated(string Name) : ToastDescriptor(ToastSeverity.Warning);