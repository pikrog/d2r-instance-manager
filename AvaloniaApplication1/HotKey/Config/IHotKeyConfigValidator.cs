using AvaloniaApplication1.HotKey.Coordination;

namespace AvaloniaApplication1.HotKey.Config;

public interface IHotKeyConfigValidator
{
    bool IsAvailable(KeyCombination combination, HotKeyCommand? command);
}