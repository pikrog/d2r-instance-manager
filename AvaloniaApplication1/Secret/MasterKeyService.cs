using System;

namespace D2RInstanceManager.Secret;

public class MasterKeyService : IMasterKeyProvider
{
    private MasterKey? _key;
    
    public MasterKey Key => _key ?? throw new InvalidOperationException("Master key hasn't been set yet");

    public void Set(MasterKey key) => _key = key;

    public void Clear()
    {
        _key?.Clear();
        _key = null;
    }
}