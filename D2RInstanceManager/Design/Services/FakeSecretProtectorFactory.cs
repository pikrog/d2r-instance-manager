using D2RInstanceManager.Secret;
using D2RInstanceManager.Secret.Protectors;

namespace D2RInstanceManager.Design.Services;

public sealed class FakeSecretProtectorFactory : ISecretProtectorFactory
{
    public ISecretProtector CreateCurrent() => new UnencryptedSecretProtector();
}