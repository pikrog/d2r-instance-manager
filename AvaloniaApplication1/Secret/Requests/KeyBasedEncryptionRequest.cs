namespace D2RInstanceManager.Secret.Requests;

public abstract record KeyBasedEncryptionRequest(string Passphrase) : EncryptionRequest;