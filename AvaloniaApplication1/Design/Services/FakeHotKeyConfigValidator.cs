using AvaloniaApplication1.HotKey;
using AvaloniaApplication1.HotKey.Config;
using AvaloniaApplication1.HotKey.Coordination;

namespace AvaloniaApplication1.Design.Services;

public class FakeHotKeyConfigValidator : IHotKeyConfigValidator
{
    public bool IsAvailable(KeyCombination combination, HotKeyCommand? command) => true;
}