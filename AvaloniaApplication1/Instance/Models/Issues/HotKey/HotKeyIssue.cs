using AvaloniaApplication1.HotKey;

namespace AvaloniaApplication1.Instance.Models.Issues.HotKey;

public abstract record HotKeyIssue(KeyCombination KeyCombination) : InstanceIssue;