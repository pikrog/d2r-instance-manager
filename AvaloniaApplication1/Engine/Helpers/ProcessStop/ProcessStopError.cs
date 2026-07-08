using AvaloniaApplication1.Engine.Platform.Process;

namespace AvaloniaApplication1.Engine.Helpers.ProcessStop;

public readonly struct ProcessStopError(bool IsTimeout, ProcessError? ProcessError)
{
    public static ProcessStopError Timeout() => new(true, null);
    
    public static ProcessStopError ProcessError(ProcessError error) => new(false, error);
}