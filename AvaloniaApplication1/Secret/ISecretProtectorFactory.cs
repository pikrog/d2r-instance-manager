using D2RInstanceManager.Secret.Protectors;

namespace D2RInstanceManager.Secret;

public interface ISecretProtectorFactory
{
    ISecretProtector CreateCurrent();
}