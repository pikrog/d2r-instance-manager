using System;
using AvaloniaApplication1.Secret.Data;

namespace AvaloniaApplication1.Account.Models;

public record AccountSnapshot(Guid Id, string? DisplayName, string Username, SecretData Password);