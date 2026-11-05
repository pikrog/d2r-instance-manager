using System.Security.Cryptography;
using AvaloniaApplication1.Secret.Options.Encryption;

namespace AvaloniaApplication1.Secret;

public sealed class MasterKeyDeriver : IMasterKeyDeriver
{
    public MasterKey Derive(string passphrase, KeyBasedEncryption options) => new(
        Rfc2898DeriveBytes.Pbkdf2(passphrase, 
            options.Salt,
            options.Iterations,
            HashAlgorithmName.SHA256,
            options.KeySize));
}