using System;

namespace AvaloniaApplication1.Account.Models;

public record AccountSummary(Guid Id, string? DisplayName, string Username, string Password, int InstanceCount);