using System;

namespace AvaloniaApplication1.Display;

public static class DisplaySelectionMapper
{
    public static DisplaySelection Map(DisplayOption option) =>
        option switch
        {
            DisplayOption.Primary => new DisplaySelection.Primary(),
            DisplayOption.Specific s => new DisplaySelection.Specific(s.Id),
            _ => throw new InvalidOperationException($"Unexpected display option: {option.GetType().Name}")
        };
}