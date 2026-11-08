using D2RInstanceManager.HotKey.Coordination;

namespace D2RInstanceManager.HotKey.Config;

public interface IHotKeyConfigValidator
{
    bool IsAvailable(KeyCombination combination, HotKeyCommand? command);
}