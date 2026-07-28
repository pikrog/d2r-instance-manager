namespace AvaloniaApplication1.Instance.Models.Issues;

public sealed record MissingRegion : GameInstanceIssue
{
    public override bool RequiresAttention => true;
}