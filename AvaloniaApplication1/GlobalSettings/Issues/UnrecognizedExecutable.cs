namespace AvaloniaApplication1.GlobalSettings.Issues;

public sealed record UnrecognizedExecutable(string Path) : GlobalSettingsIssue
{
    public override bool RequiresAttention => false;
}