using System.Collections.Generic;

namespace AvaloniaApplication1.Log;

public sealed record LogFilter(
    LogLevel Level,
    IReadOnlySet<string> InstanceNames,
    bool IncludeNoInstance,
    IReadOnlySet<string> SourceModules,
    bool IncludeNoSourceModule,
    string? SearchText);