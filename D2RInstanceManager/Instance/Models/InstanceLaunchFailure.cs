using D2RInstanceManager.Display;

namespace D2RInstanceManager.Instance.Models;

public abstract record InstanceLaunchFailure
{
    public sealed record DisplayUnavailable(string? DisplayId, DisplaySnapshot? DisplaySnapshot)
        : InstanceLaunchFailure
    {
        public bool HasNoDisplayData => DisplayId is null && DisplaySnapshot is null;
        
        public bool HasDisplayIdOnly => DisplayId is not null && DisplaySnapshot is null;
        
        public bool HasDisplaySnapshot => DisplaySnapshot is not null;
    }

    public sealed record ExecutablePathMissing : InstanceLaunchFailure
    {
        public static readonly ExecutablePathMissing Instance = new();
    }
    
    public sealed record ExecutableNotFound(string Path) : InstanceLaunchFailure;
    
    public sealed record ExecutableInvalidFormat(string Path) : InstanceLaunchFailure;
}