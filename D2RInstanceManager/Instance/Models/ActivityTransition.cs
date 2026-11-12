namespace D2RInstanceManager.Instance.Models;

public readonly record struct ActivityTransition(bool WasActive, bool IsActive, int? NewActiveCount)
{
    public bool BecameInactive => WasActive && !IsActive;
}