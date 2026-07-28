namespace AvaloniaApplication1.GlobalSettings.Issues;

public abstract record GlobalSettingsIssue
{
    public abstract bool RequiresAttention { get; }
};