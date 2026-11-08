using System;

namespace D2RInstanceManager.Account.Models;

public record AccountSummary(Guid Id, string? DisplayName, string Username, string Password, int InstanceCount);