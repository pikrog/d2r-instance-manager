namespace D2RInstanceManager.Secret;

public interface IUnlockService
{
    bool RequiresUnlock { get; }
    bool TryUnlock(string password);
}