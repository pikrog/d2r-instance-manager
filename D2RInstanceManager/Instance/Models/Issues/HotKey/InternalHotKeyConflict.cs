using D2RInstanceManager.HotKey;

namespace D2RInstanceManager.Instance.Models.Issues.HotKey;

public sealed record InternalHotKeyConflict(KeyCombination KeyCombination) : HotKeyIssue(KeyCombination)
{
    public override bool RequiresAttention => true;
}