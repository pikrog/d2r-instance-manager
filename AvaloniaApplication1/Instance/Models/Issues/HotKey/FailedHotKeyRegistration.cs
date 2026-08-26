using System;
using AvaloniaApplication1.HotKey;

namespace AvaloniaApplication1.Instance.Models.Issues.HotKey;

public sealed record FailedHotKeyRegistration(KeyCombination KeyCombination, Exception Exception) : HotKeyIssue(KeyCombination)
{
    public override bool RequiresAttention => false;
}