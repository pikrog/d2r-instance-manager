using System;
using AvaloniaApplication1.Engine.Helpers;
using AvaloniaApplication1.Engine.Models.Common;

namespace AvaloniaApplication1.Engine.Models.Contexts.Helpers;

public record ProcessStopPolicies(RetryPolicy GracefulProcessStopRetryPolicy, TimeSpan ForcefulProcessStopTimeout, uint ForcefulExitCode = RetryingProcessStopper.DefaultForcefulExitCode);