namespace D2RInstanceManager.Secret.Options.Encryption;

public sealed record DpapiCurrentUserEncryption : EncryptionOptions
{
    public static readonly DpapiCurrentUserEncryption Instance = new();
}