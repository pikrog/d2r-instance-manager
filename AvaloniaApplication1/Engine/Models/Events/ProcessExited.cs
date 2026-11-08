using D2RInstanceManager.Engine.Common;
using D2RInstanceManager.Engine.Platform.Process;

namespace D2RInstanceManager.Engine.Models.Events;

public sealed record ProcessExited(ProcessExitResult Result, ProcessError? Error = null) : Event;