using D2RInstanceManager.Secret.Options.Encryption;

namespace D2RInstanceManager.Secret;

public interface IMasterKeyDeriver
{
    public MasterKey Derive(string passphrase, KeyBasedEncryption options);
}