namespace D2RInstanceManager.GlobalSettings.Issues;

public sealed record InvalidExecutableFileFormat(string Path) : GameExecutablePathIssue
{
    public override bool RequiresAttention => true;
}