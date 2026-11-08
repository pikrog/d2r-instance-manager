namespace D2RInstanceManager.Instance.Models.Issues;

public abstract record InstanceIssue
{
    public abstract bool RequiresAttention { get; }
}