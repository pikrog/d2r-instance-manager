using System;

namespace AvaloniaApplication1.Engine.Platform.Process;

public record ProcessStartInfo
{
    public required string FileName { get; init; }
    public string Arguments { get; init; } = "";
    public IntPtr? DisplayHandle { get; init; }
}