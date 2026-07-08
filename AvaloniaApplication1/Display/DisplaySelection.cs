using System.Text.Json.Serialization;

namespace AvaloniaApplication1.Display;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Primary), "primary")]
[JsonDerivedType(typeof(Specific), "specific")]
public abstract record DisplaySelection
{
    public sealed record Primary : DisplaySelection;

    public sealed record Specific(string Id) : DisplaySelection;
};