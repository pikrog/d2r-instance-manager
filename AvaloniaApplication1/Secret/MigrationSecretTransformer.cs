using AvaloniaApplication1.Secret.Data;
using AvaloniaApplication1.Secret.Protectors;

namespace AvaloniaApplication1.Secret;

public class MigrationSecretTransformer(ISecretProtector oldProtector, ISecretProtector newProtector) : ISecretTransformer
{
    public SecretData Transform(SecretData data) => newProtector.Protect(oldProtector.Unprotect(data));
}