using AvaloniaApplication1.Secret.Data;

namespace AvaloniaApplication1.Secret.Protectors;

public sealed class UnencryptedSecretProtector : ISecretProtector
{
    public SecretData Protect(byte[] data) => new(data);

    public byte[] Unprotect(SecretData data) => data.Payload;
}