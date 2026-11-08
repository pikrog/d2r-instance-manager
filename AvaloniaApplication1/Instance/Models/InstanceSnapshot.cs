using System;
using D2RInstanceManager.Authentication.Models;
using D2RInstanceManager.Display;
using D2RInstanceManager.HotKey;

namespace D2RInstanceManager.Instance.Models;

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
    KeyCombination ShowCommandHotKey
);