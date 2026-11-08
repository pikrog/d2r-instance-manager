using System;
using D2RInstanceManager.Engine.Common;

namespace D2RInstanceManager.Engine.Helpers.ProcessStop;

public record ProcessStopPolicies(RetryPolicy GracefulProcessStopRetryPolicy, TimeSpan ForcefulProcessStopTimeout, uint ForcefulExitCode = RetryingProcessStopper.DefaultForcefulExitCode);