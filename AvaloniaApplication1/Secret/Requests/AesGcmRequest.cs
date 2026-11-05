namespace AvaloniaApplication1.Secret.Requests;

public sealed record AesGcmRequest(string Passphrase) : KeyBasedEncryptionRequest(Passphrase);