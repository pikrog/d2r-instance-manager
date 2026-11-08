using System;
using System.Collections.Generic;

namespace D2RInstanceManager.Log;

public sealed record LogSubscription(IReadOnlyList<LogEntry> Entries, IDisposable Handle) : IDisposable
{
    public void Dispose() => Handle.Dispose();
}