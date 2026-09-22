namespace AvaloniaApplication1.Engine.Common;

public readonly struct RedactedString(string value)
{
    public const string RedactedValue = "***";
    
    public string Reveal() => value;
    
    public override string ToString() => RedactedValue;
    
    public static implicit operator RedactedString(string value) => new(value);
}