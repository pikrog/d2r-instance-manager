namespace D2RInstanceManager.GlobalSettings.Issues;

public sealed record UnrecognizedExecutable(string Path) : GameExecutablePathIssue
{
    public override bool RequiresAttention => false;
}