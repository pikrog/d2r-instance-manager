namespace AvaloniaApplication1.Engine.CommandLine;

public record Argument(string Name, string? Value)
{
    public bool IsFlag => Value is null;

    public bool IsParameter => Value is not null;

    public static Argument Flag(string name) => new(name, null);

    public static Argument Parameter(string name, string value) => new(name, value);
}