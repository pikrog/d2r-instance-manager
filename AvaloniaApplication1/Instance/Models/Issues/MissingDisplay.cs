using AvaloniaApplication1.Display;

namespace AvaloniaApplication1.Instance.Models.Issues;

public sealed record MissingDisplay(CachedDisplaySnapshot Display, bool IsFallbackAllowed) : InstanceIssue
{
    public override bool RequiresAttention => !IsFallbackAllowed;
}