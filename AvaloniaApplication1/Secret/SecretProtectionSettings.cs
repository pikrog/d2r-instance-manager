using System.Text;
using D2RInstanceManager.Secret.Data;
using D2RInstanceManager.Secret.Options.Encryption;

namespace D2RInstanceManager.Secret;

public record SecretProtectionSettings
{
    private const string UnprotectedEncryptionVerificationString = "EncryptionVerificationUnprotectedData";
    
    public static readonly byte[] UnprotectedEncryptionVerificationData =
        Encoding.UTF8.GetBytes(UnprotectedEncryptionVerificationString);
    
    public EncryptionOptions EncryptionOptions { get; init; } = NoEncryption.Instance;

    public SecretData VerificationData { get; init; } = new(UnprotectedEncryptionVerificationData);
};