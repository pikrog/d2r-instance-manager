using System.Security.Cryptography;
using D2RInstanceManager.Secret.Data;

namespace D2RInstanceManager.Secret.Protectors;

public sealed class DpapiCurrentUserSecretProtector : ISecretProtector
{
#pragma warning disable CA1416
    public SecretData Protect(byte[] data)
    { 
        var ciphertext = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
        return new SecretData(ciphertext);
    }

    public byte[] Unprotect(SecretData data) =>
        ProtectedData.Unprotect(data.Payload, null, DataProtectionScope.CurrentUser);
#pragma warning restore CA1416
}