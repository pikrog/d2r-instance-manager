namespace AvaloniaApplication1.Instance.Models.Issues;

public sealed record MissingRegion : InstanceIssue
{
    public override bool RequiresAttention => true;
}