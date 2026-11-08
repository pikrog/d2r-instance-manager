namespace D2RInstanceManager.Instance.Models.Issues;

public sealed record MissingAccount : InstanceIssue
{
    public override bool RequiresAttention => true;
}