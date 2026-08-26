using AvaloniaApplication1.HotKey;

namespace AvaloniaApplication1.Instance.Models.Issues.HotKey;

public sealed record ExternalHotKeyConflict(KeyCombination KeyCombination) : HotKeyIssue(KeyCombination)
{
    public override bool RequiresAttention => false;
}