using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AvaloniaApplication1.Account;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Secret.Options.Encryption;
using AvaloniaApplication1.Secret.Options.Protector;
using AvaloniaApplication1.Secret.Requests;

namespace AvaloniaApplication1.Secret;

public class SecretMigrationCoordinator(
    ConfigService configService,
    ISecretProtectorFactory secretProtectorFactory,
    IMasterKeyDeriver keyDeriver,
    MasterKeyService masterKeyService) : ISecretMigrationCoordinator
{
    private const int SaltSize = 16;

    private (EncryptionOptions, ProtectorOptions, MasterKey) ResolveKeyBasedRequest(KeyBasedEncryptionRequest request)
    {
        var encryption = request switch
        {
            AesGcmRequest => new AesGcmEncryption(RandomNumberGenerator.GetBytes(SaltSize)),
            _ => throw new ArgumentOutOfRangeException(nameof(request))
        };
        
        var key = keyDeriver.Derive(request.Passphrase, encryption);
        
        var protectorOptions = ProtectorOptionsMapper.MapKeyBased(encryption, key);
        
        return (encryption, protectorOptions, key);
    }
    
    private (EncryptionOptions, ProtectorOptions, MasterKey?) ResolveRequest(EncryptionRequest request)
    {
        if (request is KeyBasedEncryptionRequest keyBasedRequest)
            return ResolveKeyBasedRequest(keyBasedRequest);

        EncryptionOptions encryption = request switch
        {
            NoEncryptionRequest => NoEncryption.Instance,
            DpapiCurrentUserRequest => DpapiCurrentUserEncryption.Instance,
            _ => throw new ArgumentOutOfRangeException(nameof(request))
        };
        
        var protectorOptions = ProtectorOptionsMapper.MapNonKeyBased(encryption);
        
        return (encryption, protectorOptions, null);
    }
    
    public async Task MigrateAsync(EncryptionRequest request) =>
        await configService.ChangeAsync(context =>
        {
            var currentGlobalSettings = context.GetGlobalSettings();
            
            var oldProtector = secretProtectorFactory.CreateCurrent();
            
            var (newEncryption, newProtectorOptions, key) = ResolveRequest(request);

            var newProtector = SecretProtectorFactory.Create(newProtectorOptions);
            
            var secretTransformer = new MigrationSecretTransformer(oldProtector, newProtector);
            
            AccountSecretMigrationService.Migrate(context, secretTransformer);
            
            var verificationData = 
                newProtector.Protect(SecretProtectionSettings.UnprotectedEncryptionVerificationData);
            
            var newSecretProtection = new SecretProtectionSettings
            {
                EncryptionOptions = newEncryption,
                VerificationData = verificationData,
            };
            
            var newGlobalSettings = currentGlobalSettings with { SecretProtection = newSecretProtection };
            context.UpdateGlobalSettings(newGlobalSettings);
            
            if (key is not null)
                masterKeyService.Set(key);
            else
                masterKeyService.Clear();
        });
}