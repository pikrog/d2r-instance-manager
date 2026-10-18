namespace AvaloniaApplication1.Instance.Models.Issues;

public sealed record InvalidDisplay : InstanceIssue
{
    public override bool RequiresAttention => true;
}