using System;

namespace AvaloniaApplication1.Account.Models;

public record AccountSnapshot(Guid Id, string? DisplayName, string Username, string Password);