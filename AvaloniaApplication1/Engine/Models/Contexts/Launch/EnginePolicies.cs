using AvaloniaApplication1.Engine.Common;
using AvaloniaApplication1.Engine.Helpers.ProcessStop;

namespace AvaloniaApplication1.Engine.Models.Contexts.Launch;

public record EnginePolicies
(
    RetryPolicy UnlockMultiboxRetryPolicy,
    ProcessStopPolicies ProcessStopPolicies
);