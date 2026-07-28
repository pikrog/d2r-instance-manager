namespace AvaloniaApplication1.GlobalSettings.Issues;

public sealed record MissingExecutablePath : GlobalSettingsIssue
{
    public override bool RequiresAttention => true;
}