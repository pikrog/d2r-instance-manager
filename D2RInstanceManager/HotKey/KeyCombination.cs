using Avalonia.Input;
using D2RInstanceManager.Common;

namespace D2RInstanceManager.HotKey;

public record KeyCombination(Key Key, KeyModifiers KeyModifiers = KeyModifiers.None)
{
    public override string ToString() => KeysStringifier.ToString(Key, KeyModifiers);
}