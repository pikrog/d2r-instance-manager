using D2RInstanceManager.HotKey;

namespace D2RInstanceManager.Instance.Models.Issues.HotKey;

public sealed record ExternalHotKeyConflict(KeyCombination KeyCombination) : HotKeyIssue(KeyCombination)
{
    public override bool RequiresAttention => false;
}