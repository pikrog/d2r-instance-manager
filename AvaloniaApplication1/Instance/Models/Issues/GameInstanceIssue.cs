namespace AvaloniaApplication1.Instance.Models.Issues;

public abstract record GameInstanceIssue
{
    public abstract bool RequiresAttention { get; }
}