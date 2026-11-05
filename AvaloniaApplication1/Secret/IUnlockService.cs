namespace AvaloniaApplication1.Secret;

public interface IUnlockService
{
    bool RequiresUnlock { get; }
    bool TryUnlock(string password);
}