using System;

namespace D2RInstanceManager.Toast;

public static class ToastDurations
{
    public static readonly TimeSpan Info = TimeSpan.FromSeconds(4);
    public static readonly TimeSpan Success = TimeSpan.FromSeconds(3);
    public static readonly TimeSpan Warning = TimeSpan.FromSeconds(6);
    public static readonly TimeSpan Error = TimeSpan.FromSeconds(8);

    public static TimeSpan Get(ToastSeverity severity) =>
        severity switch
        {
            ToastSeverity.Info => Info,
            ToastSeverity.Success => Success,
            ToastSeverity.Warning => Warning,
            ToastSeverity.Error => Error,
            _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
        };
}