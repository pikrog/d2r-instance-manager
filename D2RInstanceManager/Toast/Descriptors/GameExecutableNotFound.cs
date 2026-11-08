namespace D2RInstanceManager.Toast.Descriptors;

public sealed record GameExecutableNotFound() : ToastDescriptor(ToastSeverity.Error)
{
    public static readonly GameExecutableNotFound Instance = new();
}