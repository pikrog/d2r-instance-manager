namespace D2RInstanceManager.GlobalSettings.Issues;

public abstract record GlobalSettingsIssue
{
    public abstract bool RequiresAttention { get; }
};