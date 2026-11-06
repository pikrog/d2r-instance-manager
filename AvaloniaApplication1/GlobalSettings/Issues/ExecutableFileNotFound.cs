namespace AvaloniaApplication1.GlobalSettings.Issues;

public sealed record ExecutableFileNotFound(string Path) : GameExecutablePathIssue
{
    public override bool RequiresAttention => true;
}