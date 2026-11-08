namespace D2RInstanceManager.GlobalSettings.Issues;

public sealed record MissingExecutablePath : GameExecutablePathIssue
{
    public override bool RequiresAttention => true;
}