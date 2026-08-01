namespace AvaloniaApplication1.Instance.Models.Issues;

public abstract record InstanceIssue
{
    public abstract bool RequiresAttention { get; }
}