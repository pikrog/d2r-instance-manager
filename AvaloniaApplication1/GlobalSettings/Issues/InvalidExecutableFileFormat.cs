namespace AvaloniaApplication1.GlobalSettings.Issues;

public sealed record InvalidExecutableFileFormat(string Path) : GlobalSettingsIssue
{
    public override bool RequiresAttention => true;
}