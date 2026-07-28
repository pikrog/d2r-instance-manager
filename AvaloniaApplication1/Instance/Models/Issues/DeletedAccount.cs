namespace AvaloniaApplication1.Instance.Models.Issues;

public sealed record DeletedAccount : GameInstanceIssue
{
    public override bool RequiresAttention => true;
};