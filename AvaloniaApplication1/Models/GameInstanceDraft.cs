using System;

namespace AvaloniaApplication1.Models;

public record GameInstanceDraft(
    Guid? Id,
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