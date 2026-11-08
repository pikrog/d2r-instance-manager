using System.Collections.Generic;

namespace D2RInstanceManager.Log;

public sealed record LogFilter(
    LogLevel Level,
    IReadOnlySet<string> InstanceNames,
    bool IncludeNoInstance,
    IReadOnlySet<string> SourceModules,
    bool IncludeNoSourceModule,
    string? SearchText);