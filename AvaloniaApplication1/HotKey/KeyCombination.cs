using Avalonia.Input;
using AvaloniaApplication1.Common;

namespace AvaloniaApplication1.HotKey;

public record KeyCombination(Key Key, KeyModifiers KeyModifiers)
{
    public override string ToString() => KeysStringifier.ToString(Key, KeyModifiers);
}