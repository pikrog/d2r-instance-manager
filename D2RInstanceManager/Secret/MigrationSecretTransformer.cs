using D2RInstanceManager.Secret.Data;
using D2RInstanceManager.Secret.Protectors;

namespace D2RInstanceManager.Secret;

public class MigrationSecretTransformer(ISecretProtector oldProtector, ISecretProtector newProtector) : ISecretTransformer
{
    public SecretData Transform(SecretData data) => newProtector.Protect(oldProtector.Unprotect(data));
}