using D2RInstanceManager.Display;

namespace D2RInstanceManager.Instance.Models.Issues;

public sealed record MissingDisplay(DisplaySnapshot Display, bool IsFallbackAllowed) : InstanceIssue
{
    public override bool RequiresAttention => !IsFallbackAllowed;
}