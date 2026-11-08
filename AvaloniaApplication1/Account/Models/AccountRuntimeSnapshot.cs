using System;

namespace D2RInstanceManager.Account.Models;

public record AccountRuntimeSnapshot(Guid Id, string? DisplayName, string Username, string Password);