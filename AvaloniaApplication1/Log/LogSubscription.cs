using System;
using System.Collections.Generic;

namespace AvaloniaApplication1.Log;

public sealed record LogSubscription(IReadOnlyList<LogEntry> Entries, IDisposable Handle) : IDisposable
{
    public void Dispose() => Handle.Dispose();
}