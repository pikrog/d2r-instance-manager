namespace D2RInstanceManager.Toast.Descriptors;

public sealed record GameExecutableFound(string Path) : ToastDescriptor(ToastSeverity.Success);