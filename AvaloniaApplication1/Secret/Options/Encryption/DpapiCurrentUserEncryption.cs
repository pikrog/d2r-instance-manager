namespace AvaloniaApplication1.Secret.Options.Encryption;

public sealed record DpapiCurrentUserEncryption : EncryptionOptions
{
    public static readonly DpapiCurrentUserEncryption Instance = new();
}