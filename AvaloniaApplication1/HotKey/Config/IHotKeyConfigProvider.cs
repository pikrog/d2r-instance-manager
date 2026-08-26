using System.Collections.Generic;
using AvaloniaApplication1.HotKey.Coordination;

namespace AvaloniaApplication1.HotKey.Config;

public interface IHotKeyConfigProvider
{
    IReadOnlyList<HotKeyBinding> GetBindings();
}

public interface IHotKeyConfigProvider<T> : IHotKeyConfigProvider where T : HotKeyCommand
{
    new IReadOnlyList<HotKeyBinding<T>> GetBindings();

    IReadOnlyList<HotKeyBinding> IHotKeyConfigProvider.GetBindings() => GetBindings();
}