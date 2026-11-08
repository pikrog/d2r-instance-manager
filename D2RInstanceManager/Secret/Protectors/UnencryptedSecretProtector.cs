using D2RInstanceManager.Secret.Data;

namespace D2RInstanceManager.Secret.Protectors;

public sealed class UnencryptedSecretProtector : ISecretProtector
{
    public SecretData Protect(byte[] data) => new(data);

    public byte[] Unprotect(SecretData data) => data.Payload;
}