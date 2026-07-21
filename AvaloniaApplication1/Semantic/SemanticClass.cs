using Avalonia;

namespace AvaloniaApplication1.Semantic;

public class SemanticClass : AvaloniaObject
{
    public static readonly AttachedProperty<SemanticType?> TypeProperty = 
        AvaloniaProperty.RegisterAttached<SemanticClass, StyledElement, SemanticType?>("Type");
    
    public static SemanticType? GetType(StyledElement element) => element.GetValue(TypeProperty);
    
    public static void SetType(StyledElement element, SemanticType? value) => element.SetValue(TypeProperty, value);

    static SemanticClass()
    {
        TypeProperty.Changed.AddClassHandler<StyledElement>(OnTypeChanged);
    }

    private static void OnTypeChanged(StyledElement element, AvaloniaPropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.OldValue is SemanticType oldType)
        {
            var oldTypeClass = SemanticClassMapper.Map(oldType);
            element.Classes.Remove(oldTypeClass);
        }
        
        if (eventArgs.NewValue is SemanticType newType)
        {
            var newTypeClass = SemanticClassMapper.Map(newType);
            element.Classes.Add(newTypeClass);
        }
    }
}