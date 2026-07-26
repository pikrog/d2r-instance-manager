using AvaloniaApplication1.Engine.Platform.Process;

namespace AvaloniaApplication1.Engine.Helpers.ProcessStop.Error;

public sealed record ProcessStopOperationError(ProcessError Error) : ProcessStopError;