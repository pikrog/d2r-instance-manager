using System;
using D2RInstanceManager.Secret.Options.Encryption;
using D2RInstanceManager.Secret.Options.Protector;

namespace D2RInstanceManager.Secret;

public static class ProtectorOptionsMapper
{
    public static ProtectorOptions MapNonKeyBased(
        EncryptionOptions encryptionOptions) => encryptionOptions switch
    {
        NoEncryption => UnencryptedProtectorOptions.Instance,

        DpapiCurrentUserEncryption => DpapiCurrentUserProtectorOptions.Instance,

        KeyBasedEncryption =>
            throw new NotSupportedException("This method does not map key-based encryption options"),

        _ => throw new ArgumentOutOfRangeException(nameof(encryptionOptions))
    };

    public static ProtectorOptions MapKeyBased(KeyBasedEncryption encryptionOptions,
        MasterKey key) => encryptionOptions switch
    {
        AesGcmEncryption => new AesGcmProtectorOptions(key),
        _ => throw new ArgumentOutOfRangeException(nameof(encryptionOptions))
    };
}