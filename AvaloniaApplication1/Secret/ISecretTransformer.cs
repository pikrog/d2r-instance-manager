using AvaloniaApplication1.Secret.Data;

namespace AvaloniaApplication1.Secret;

public interface ISecretTransformer
{
    SecretData Transform(SecretData data);
}