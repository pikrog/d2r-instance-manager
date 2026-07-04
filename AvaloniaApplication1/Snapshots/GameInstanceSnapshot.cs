using System;
using Avalonia.Input;
using AvaloniaApplication1.Models;

namespace AvaloniaApplication1.Snapshots;

public record GameInstanceSnapshot
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
    HotKey RecallHotKey
);