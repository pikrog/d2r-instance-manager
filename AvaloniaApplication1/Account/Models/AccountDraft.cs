using System;

namespace AvaloniaApplication1.Account.Models;

public record AccountDraft(Guid? Id, string? DisplayName, string Username, string Password);