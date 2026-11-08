using System.Linq;
using System.Security.Cryptography;
using D2RInstanceManager.Config;
using D2RInstanceManager.Secret.Data;
using D2RInstanceManager.Secret.Options.Encryption;
using D2RInstanceManager.Secret.Options.Protector;

namespace D2RInstanceManager.Secret;

public class SecretProtectionVerifier(ConfigService configService) : INonKeyBasedSecretProtectionVerifier
{
    private static bool Verify(ProtectorOptions options, SecretData verificationData)
    {
        var protector = SecretProtectorFactory.Create(options);
        
        try
        {
            var decryptedVerificationData = protector.Unprotect(verificationData);
            
            return decryptedVerificationData.SequenceEqual(
                SecretProtectionSettings.UnprotectedEncryptionVerificationData);
        }
        catch (CryptographicException)
        {
            return false;
        }
    }
    
    public static bool VerifyKeyBased(KeyBasedEncryption encryptionOptions,
        MasterKey key,
        SecretData verificationData)
    {
        var protectorOptions = ProtectorOptionsMapper.MapKeyBased(encryptionOptions, key);
        
        return Verify(protectorOptions, verificationData);
    }

    public bool VerifyNonKeyBased()
    {
        var protectionSettings = configService.Config.GetGlobalSettings().SecretProtection;

        var protectorOptions = ProtectorOptionsMapper.MapNonKeyBased(protectionSettings.EncryptionOptions);

        return Verify(protectorOptions, protectionSettings.VerificationData);
    }
}