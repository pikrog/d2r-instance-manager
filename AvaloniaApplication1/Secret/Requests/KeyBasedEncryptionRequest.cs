namespace AvaloniaApplication1.Secret.Requests;

public abstract record KeyBasedEncryptionRequest(string Passphrase) : EncryptionRequest;