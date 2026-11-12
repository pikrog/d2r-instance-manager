using D2RInstanceManager.Engine.Models.Events;

namespace D2RInstanceManager.Toast.Descriptors;

public sealed record InstanceFailed(string Name, ErrorEvent Error) : ToastDescriptor(ToastSeverity.Error);