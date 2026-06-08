using System.ComponentModel.DataAnnotations;

namespace AvaloniaApplication1.Attributes;

public class RequiredIfAttribute(string propertyName, object targetValue) : RequiredAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var propertyValue = validationContext.ObjectType
            .GetProperty(propertyName)
            ?.GetValue(validationContext.ObjectInstance);

        if (propertyValue is null || !propertyValue.Equals(targetValue))
            return ValidationResult.Success;
        
        return base.IsValid(value, validationContext);
    }
    
}