namespace AvaloniaApplication1.Instance.Models.Issues;

public sealed record MissingAccount : GameInstanceIssue
{
    public override bool RequiresAttention => true;
}