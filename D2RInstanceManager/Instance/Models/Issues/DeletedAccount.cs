namespace D2RInstanceManager.Instance.Models.Issues;

public sealed record DeletedAccount : InstanceIssue
{
    public override bool RequiresAttention => true;
};