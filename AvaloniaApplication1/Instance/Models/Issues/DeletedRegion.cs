namespace AvaloniaApplication1.Instance.Models.Issues;

public sealed record DeletedRegion : GameInstanceIssue
{
    public override bool RequiresAttention => true;
}