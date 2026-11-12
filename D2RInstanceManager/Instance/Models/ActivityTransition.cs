namespace D2RInstanceManager.Instance.Models;

public readonly record struct ActivityTransition(
    bool WasActive,
    bool IsActive,
    int? NewActiveCount,
    bool IsIgnored = false)
{
    public bool BecameInactive => WasActive && !IsActive;

    public static ActivityTransition Ignored => new(false, false, null, true);
}