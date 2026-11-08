using System;
using D2RInstanceManager.HotKey;

namespace D2RInstanceManager.Instance.Models.Issues.HotKey;

public sealed record FailedHotKeyRegistration(KeyCombination KeyCombination, Exception Exception) : HotKeyIssue(KeyCombination)
{
    public override bool RequiresAttention => false;
}