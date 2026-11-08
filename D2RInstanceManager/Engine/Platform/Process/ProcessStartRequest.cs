using System;

namespace D2RInstanceManager.Engine.Platform.Process;

public record ProcessStartRequest
{
    public required string FileName { get; init; }
    public string Arguments { get; init; } = "";
    public IntPtr? DisplayHandle { get; init; }
}