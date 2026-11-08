namespace D2RInstanceManager.Secret.Requests;

public sealed record AesGcmRequest(string Passphrase) : KeyBasedEncryptionRequest(Passphrase);