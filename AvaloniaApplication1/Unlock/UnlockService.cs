using System;
using D2RInstanceManager.Config;
using D2RInstanceManager.Secret;
using D2RInstanceManager.Secret.Options.Encryption;

namespace D2RInstanceManager.Unlock;

public class UnlockService(ConfigService configService, MasterKeyService keyService, IMasterKeyDeriver keyDeriver)
    : IUnlockService
{
    private bool _isUnlocked;

    private SecretProtectionSettings GetSecretProtectionSettings() =>
        configService.Config.GetGlobalSettings().SecretProtection;

    public bool RequiresUnlock => 
        !_isUnlocked && GetSecretProtectionSettings().EncryptionOptions is KeyBasedEncryption;
    
    public bool TryUnlock(string password)
    {
        _isUnlocked = false;
        keyService.Clear();
        
        var protectionSettings = GetSecretProtectionSettings();

        if (protectionSettings.EncryptionOptions is not KeyBasedEncryption options)
            throw new InvalidOperationException("Attempted to unlock with a non-key-based encryption method.");
        
        var key = keyDeriver.Derive(password, options);

        if (!SecretProtectionVerifier.VerifyKeyBased(options, key, protectionSettings.VerificationData))
            return false;
        
        keyService.Set(key);
        _isUnlocked = true;
        
        return true;
    }
}