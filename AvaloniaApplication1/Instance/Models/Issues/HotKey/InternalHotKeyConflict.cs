using AvaloniaApplication1.HotKey;

namespace AvaloniaApplication1.Instance.Models.Issues.HotKey;

public sealed record InternalHotKeyConflict(KeyCombination KeyCombination) : HotKeyIssue(KeyCombination)
{
    public override bool RequiresAttention => true;
}