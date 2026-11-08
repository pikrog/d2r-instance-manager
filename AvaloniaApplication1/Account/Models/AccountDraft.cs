using System;

namespace D2RInstanceManager.Account.Models;

public record AccountDraft(Guid? Id, string? DisplayName, string Username, string Password);