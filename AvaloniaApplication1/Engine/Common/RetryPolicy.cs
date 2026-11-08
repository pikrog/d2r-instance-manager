using System;

namespace D2RInstanceManager.Engine.Common;

public record RetryPolicy(TimeSpan Delay, int MaxRetries);