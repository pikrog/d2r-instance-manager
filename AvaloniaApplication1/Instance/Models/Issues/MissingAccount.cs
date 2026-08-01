namespace AvaloniaApplication1.Instance.Models.Issues;

public sealed record MissingAccount : InstanceIssue
{
    public override bool RequiresAttention => true;
}