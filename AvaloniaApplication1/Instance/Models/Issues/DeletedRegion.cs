namespace AvaloniaApplication1.Instance.Models.Issues;

public sealed record DeletedRegion : InstanceIssue
{
    public override bool RequiresAttention => true;
}