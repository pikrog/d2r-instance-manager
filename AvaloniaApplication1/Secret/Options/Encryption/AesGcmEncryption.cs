namespace AvaloniaApplication1.Secret.Options.Encryption;

public sealed record AesGcmEncryption(byte[] Salt) : KeyBasedEncryption(Salt, 32, 600000);