namespace AvaloniaApplication1.GlobalSettings.Issues;

public sealed record ExecutableFileNotFound(string Path) : GlobalSettingsIssue
{
    public override bool RequiresAttention => true;
}