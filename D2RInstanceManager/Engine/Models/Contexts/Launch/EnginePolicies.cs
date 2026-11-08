using D2RInstanceManager.Engine.Common;
using D2RInstanceManager.Engine.Helpers.ProcessStop;

namespace D2RInstanceManager.Engine.Models.Contexts.Launch;

public record EnginePolicies
(
    RetryPolicy UnlockMultiboxRetryPolicy,
    ProcessStopPolicies ProcessStopPolicies
);