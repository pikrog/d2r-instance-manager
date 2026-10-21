namespace AvaloniaApplication1.Toast.Descriptors;

public sealed record GameExecutableFound(string Path) : ToastDescriptor(ToastSeverity.Success);