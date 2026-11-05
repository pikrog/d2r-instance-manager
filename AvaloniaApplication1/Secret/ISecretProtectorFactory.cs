using AvaloniaApplication1.Secret.Protectors;

namespace AvaloniaApplication1.Secret;

public interface ISecretProtectorFactory
{
    ISecretProtector CreateCurrent();
}