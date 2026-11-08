using System;
using D2RInstanceManager.Secret.Data;

namespace D2RInstanceManager.Account.Models;

public record AccountSnapshot(Guid Id, string? DisplayName, string Username, SecretData Password);