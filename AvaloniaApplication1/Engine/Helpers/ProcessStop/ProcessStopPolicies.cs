using System;
using AvaloniaApplication1.Engine.Common;

namespace AvaloniaApplication1.Engine.Helpers.ProcessStop;

public record ProcessStopPolicies(RetryPolicy GracefulProcessStopRetryPolicy, TimeSpan ForcefulProcessStopTimeout, uint ForcefulExitCode = RetryingProcessStopper.DefaultForcefulExitCode);