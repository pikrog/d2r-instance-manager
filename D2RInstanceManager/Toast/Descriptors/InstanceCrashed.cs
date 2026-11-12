namespace D2RInstanceManager.Toast.Descriptors;

public sealed record InstanceCrashed(string Name) : ToastDescriptor(ToastSeverity.Error);