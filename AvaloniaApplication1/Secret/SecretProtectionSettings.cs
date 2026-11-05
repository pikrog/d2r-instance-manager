using System.Text;
using AvaloniaApplication1.Secret.Data;
using AvaloniaApplication1.Secret.Options.Encryption;

namespace AvaloniaApplication1.Secret;

public record SecretProtectionSettings
{
    private const string UnprotectedEncryptionVerificationString = "EncryptionVerificationUnprotectedData";
    
    public static readonly byte[] UnprotectedEncryptionVerificationData =
        Encoding.UTF8.GetBytes(UnprotectedEncryptionVerificationString);
    
    public EncryptionOptions EncryptionOptions { get; init; } = NoEncryption.Instance;

    public SecretData VerificationData { get; init; } = new(UnprotectedEncryptionVerificationData);
};