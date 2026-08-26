using System.Collections.Generic;
using Avalonia.Input;
using Avalonia.Win32.Input;

namespace AvaloniaApplication1.Common;

public static class KeysStringifier
{
    public static string ToString(Key key, KeyModifiers keyModifiers)
    {
        var modifiers = new List<string>();
        if (keyModifiers.HasFlag(KeyModifiers.Control))
            modifiers.Add("Ctrl");
        if (keyModifiers.HasFlag(KeyModifiers.Shift))
            modifiers.Add("Shift");
        if (keyModifiers.HasFlag(KeyModifiers.Alt))
            modifiers.Add("Alt");
        if (keyModifiers.HasFlag(KeyModifiers.Meta))
            modifiers.Add("Win");
        
        var virtualKey = KeyInterop.VirtualKeyFromKey(key);
        var keySymbol = KeyInterop.GetKeySymbol(virtualKey, 0) ?? key.ToString();
        
        return modifiers.Count > 0
            ? $"{string.Join(" + ", modifiers)} + {keySymbol}"
            : keySymbol;
    }
}