using System;

namespace AvaloniaApplication1.Account.Models;

public record AccountRuntimeSnapshot(Guid Id, string? DisplayName, string Username, string Password);