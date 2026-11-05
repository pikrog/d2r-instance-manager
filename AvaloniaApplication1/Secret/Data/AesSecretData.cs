namespace AvaloniaApplication1.Secret.Data;

public sealed record AesSecretData(byte[] Payload, byte[] Nonce, byte[] Tag) : SecretData(Payload);