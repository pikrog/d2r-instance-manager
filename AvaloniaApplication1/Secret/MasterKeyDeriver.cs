using System.Security.Cryptography;
using D2RInstanceManager.Secret.Options.Encryption;

namespace D2RInstanceManager.Secret;

public sealed class MasterKeyDeriver : IMasterKeyDeriver
{
    public MasterKey Derive(string passphrase, KeyBasedEncryption options) => new(
        Rfc2898DeriveBytes.Pbkdf2(passphrase, 
            options.Salt,
            options.Iterations,
            HashAlgorithmName.SHA256,
            options.KeySize));
}