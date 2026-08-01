using System;
using AvaloniaApplication1.Authentication.Models;
using AvaloniaApplication1.Display;

namespace AvaloniaApplication1.Instance.Models;

public record InstanceSnapshot
(
    Guid Id,
    string Name,
    bool IsOnlineMode,
    Guid? AccountId,
    AuthenticationMethod? AuthenticationMethod,
    Guid? RegionId,
    DisplaySelection Display,
    bool IsNoSound,
    bool IsWindowedMode,
    HotKey.HotKey RecallHotKey
);