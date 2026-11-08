using D2RInstanceManager.HotKey;

namespace D2RInstanceManager.Instance.Models.Issues.HotKey;

public abstract record HotKeyIssue(KeyCombination KeyCombination) : InstanceIssue;