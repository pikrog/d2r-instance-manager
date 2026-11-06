namespace AvaloniaApplication1.GlobalSettings.Issues;

public sealed record MissingExecutablePath : GameExecutablePathIssue
{
    public override bool RequiresAttention => true;
}