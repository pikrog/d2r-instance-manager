using System;

namespace AvaloniaApplication1.Account.Models;

public record AccountDraft(Guid? Id, string Username, string Password);