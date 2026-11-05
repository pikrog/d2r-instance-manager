using AvaloniaApplication1.Secret;
using AvaloniaApplication1.Secret.Protectors;

namespace AvaloniaApplication1.Design.Services;

public sealed class FakeSecretProtectorFactory : ISecretProtectorFactory
{
    public ISecretProtector CreateCurrent() => new UnencryptedSecretProtector();
}