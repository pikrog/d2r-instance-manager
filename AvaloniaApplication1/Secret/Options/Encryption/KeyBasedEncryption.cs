namespace AvaloniaApplication1.Secret.Options.Encryption;

public abstract record KeyBasedEncryption(byte[] Salt, int KeySize, int Iterations) : EncryptionOptions;