using System.Collections.Generic;
using System.Linq;
using AvaloniaApplication1.HotKey.Coordination;

namespace AvaloniaApplication1.HotKey.Config;

public class HotKeyConfigValidator(IEnumerable<IHotKeyConfigProvider> providers) : IHotKeyConfigValidator
{
    public bool IsAvailable(KeyCombination combination, HotKeyCommand? command) => 
        !providers.SelectMany(p => p.GetBindings())
            .Any(b => b.KeyCombination == combination && b.Command != command);
}