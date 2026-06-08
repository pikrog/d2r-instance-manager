using System;

namespace AvaloniaApplication1.Engine.Models.Common;

public record RetryPolicy(TimeSpan Delay, int MaxRetries);