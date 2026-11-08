namespace D2RInstanceManager.Secret.Options.Protector;

public sealed record UnencryptedProtectorOptions : ProtectorOptions
{
    public static readonly UnencryptedProtectorOptions Instance = new();
}