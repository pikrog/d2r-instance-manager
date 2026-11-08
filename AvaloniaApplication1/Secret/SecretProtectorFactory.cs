using System;
using D2RInstanceManager.Config;
using D2RInstanceManager.Secret.Options.Encryption;
using D2RInstanceManager.Secret.Options.Protector;
using D2RInstanceManager.Secret.Protectors;

namespace D2RInstanceManager.Secret;

public class SecretProtectorFactory(ConfigService configService, IMasterKeyProvider keyProvider) 
    : ISecretProtectorFactory
{
    public ISecretProtector CreateCurrent()
    {
        var encryptionOptions = configService.Config.GetGlobalSettings().SecretProtection.EncryptionOptions;

        var protectorOptions = encryptionOptions switch
        {
            KeyBasedEncryption keyBased => ProtectorOptionsMapper.MapKeyBased(keyBased, keyProvider.Key),
            _ => ProtectorOptionsMapper.MapNonKeyBased(encryptionOptions)
        };
        
        return Create(protectorOptions);
    }

    public static ISecretProtector Create(ProtectorOptions options) => options switch
    {
        UnencryptedProtectorOptions => new UnencryptedSecretProtector(),
        DpapiCurrentUserProtectorOptions => new DpapiCurrentUserSecretProtector(),
        AesGcmProtectorOptions aes => new AesGcmSecretProtector(aes.Key.Bytes.ToArray()),
        _ => throw new ArgumentOutOfRangeException(nameof(options))
    };
}