using System;
using System.ComponentModel.DataAnnotations;
using Avalonia.Input;
using AvaloniaApplication1.HotKey;
using AvaloniaApplication1.HotKey.Config;
using AvaloniaApplication1.HotKey.Coordination;
using AvaloniaApplication1.Languages;

namespace AvaloniaApplication1.Common;

public sealed class NotBoundHotKeyAttribute(string? identityPropertyName = null) : ValidationAttribute(Validation.NotBoundHotKeyAttribute_ErrorMessage)
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not KeyCombination keyCombination || keyCombination.Key == Key.None)
            return ValidationResult.Success;

        var validator = validationContext.GetService(typeof(IHotKeyConfigValidator)) as IHotKeyConfigValidator
                        ?? throw new InvalidOperationException($"{nameof(IHotKeyConfigValidator)} is not available in validation context.");
        
        HotKeyCommand? excludedCommand = null;
        if (identityPropertyName is not null)
        {
            var propertyInfo = validationContext.ObjectType.GetProperty(identityPropertyName);
            if (propertyInfo is not null)
                excludedCommand = propertyInfo.GetValue(validationContext.ObjectInstance) as HotKeyCommand;
        }
        
        return validator.IsAvailable(keyCombination, excludedCommand)
            ? ValidationResult.Success
            : new ValidationResult(ErrorMessageString);
    }
}