namespace D2RInstanceManager.Secret.Options.Encryption;

public abstract record KeyBasedEncryption(byte[] Salt, int KeySize, int Iterations) : EncryptionOptions;