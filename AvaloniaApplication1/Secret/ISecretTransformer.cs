using D2RInstanceManager.Secret.Data;

namespace D2RInstanceManager.Secret;

public interface ISecretTransformer
{
    SecretData Transform(SecretData data);
}