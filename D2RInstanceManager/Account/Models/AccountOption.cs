using System;

namespace D2RInstanceManager.Account.Models;

public record AccountOption(Guid Id, string? DisplayName, string Username)
{
    public string Label => string.IsNullOrWhiteSpace(DisplayName) 
        ? Username 
        : $"{DisplayName} ({Username})";
}