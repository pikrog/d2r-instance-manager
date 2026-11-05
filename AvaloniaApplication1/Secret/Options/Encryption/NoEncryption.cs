namespace AvaloniaApplication1.Secret.Options.Encryption;

public sealed record NoEncryption : EncryptionOptions
{
    public static readonly NoEncryption Instance = new();
}