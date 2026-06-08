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
    CredentialsVector? CredentialsVector,
    Guid? RegionId,
    int DisplayId,
    bool IsNoSound,
    bool IsWindowedMode,
    HotKey RecallHotKey
);