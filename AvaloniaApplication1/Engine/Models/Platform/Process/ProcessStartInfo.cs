using System;

namespace AvaloniaApplication1.Engine.Models.Platform.Process;

public record ProcessStartInfo
{
    public required string ExecutablePath { get; init; }
    public string Arguments { get; init; } = "";
    public IntPtr? DisplayHandle { get; init; }
}