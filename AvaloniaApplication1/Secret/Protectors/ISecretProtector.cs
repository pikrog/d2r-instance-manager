using D2RInstanceManager.Secret.Data;

namespace D2RInstanceManager.Secret.Protectors;

public interface ISecretProtector
{
    SecretData Protect(byte[] data);
    byte[] Unprotect(SecretData data);
}