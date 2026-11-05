using AvaloniaApplication1.Secret.Options.Encryption;

namespace AvaloniaApplication1.Secret;

public interface IMasterKeyDeriver
{
    public MasterKey Derive(string passphrase, KeyBasedEncryption options);
}