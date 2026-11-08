using D2RInstanceManager.Engine.Platform.Process;

namespace D2RInstanceManager.Engine.Helpers.ProcessStop.Error;

public sealed record ProcessStopOperationError(ProcessError Error) : ProcessStopError;