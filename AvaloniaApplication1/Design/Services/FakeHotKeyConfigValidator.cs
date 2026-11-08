using D2RInstanceManager.HotKey;
using D2RInstanceManager.HotKey.Config;
using D2RInstanceManager.HotKey.Coordination;

namespace D2RInstanceManager.Design.Services;

public class FakeHotKeyConfigValidator : IHotKeyConfigValidator
{
    public bool IsAvailable(KeyCombination combination, HotKeyCommand? command) => true;
}