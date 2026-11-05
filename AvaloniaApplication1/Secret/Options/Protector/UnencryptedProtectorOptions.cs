namespace AvaloniaApplication1.Secret.Options.Protector;

public sealed record UnencryptedProtectorOptions : ProtectorOptions
{
    public static readonly UnencryptedProtectorOptions Instance = new();
}