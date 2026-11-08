using System.Collections.Generic;
using System.Linq;
using D2RInstanceManager.HotKey.Coordination;

namespace D2RInstanceManager.HotKey.Config;

public class HotKeyConfigValidator(IEnumerable<IHotKeyConfigProvider> providers) : IHotKeyConfigValidator
{
    public bool IsAvailable(KeyCombination combination, HotKeyCommand? command) => 
        !providers.SelectMany(p => p.GetBindings())
            .Any(b => b.KeyCombination == combination && b.Command != command);
}