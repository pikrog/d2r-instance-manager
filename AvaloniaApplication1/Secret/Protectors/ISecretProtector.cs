using AvaloniaApplication1.Secret.Data;

namespace AvaloniaApplication1.Secret.Protectors;

public interface ISecretProtector
{
    SecretData Protect(byte[] data);
    byte[] Unprotect(SecretData data);
}