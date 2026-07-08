using System;

namespace AvaloniaApplication1.Engine.Common;

public record RetryPolicy(TimeSpan Delay, int MaxRetries);