using System;

namespace AvaloniaApplication1.Models;

public record GameInstanceDraft(
    Guid? Id,
    string Name,
    bool IsOnlineMode,
    Guid? AccountId,
    CredentialsVector? CredentialsVector,
    Guid? RegionId,
    DisplaySelection Display,
    bool IsNoSound,
    bool IsWindowedMode,
    HotKey RecallHotKey
    );