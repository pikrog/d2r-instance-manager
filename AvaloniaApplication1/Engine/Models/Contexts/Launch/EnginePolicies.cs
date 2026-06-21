using System;
using AvaloniaApplication1.Engine.Models.Common;
using AvaloniaApplication1.Engine.Models.Contexts.Helpers;

namespace AvaloniaApplication1.Engine.Models.Contexts.Launch;

public record EnginePolicies
(
    RetryPolicy UnlockMultiboxRetryPolicy,
    ProcessStopPolicies ProcessStopPolicies
);