using System.Collections.Generic;
using D2RInstanceManager.HotKey.Coordination;

namespace D2RInstanceManager.HotKey.Config;

public interface IHotKeyConfigProvider
{
    IReadOnlyList<HotKeyBinding> GetBindings();
}

public interface IHotKeyConfigProvider<T> : IHotKeyConfigProvider where T : HotKeyCommand
{
    new IReadOnlyList<HotKeyBinding<T>> GetBindings();

    IReadOnlyList<HotKeyBinding> IHotKeyConfigProvider.GetBindings() => GetBindings();
}