namespace AvaloniaApplication1.Toast.Descriptors;

public sealed record GameExecutableNotFound() : ToastDescriptor(ToastSeverity.Error)
{
    public static readonly GameExecutableNotFound Instance = new();
}