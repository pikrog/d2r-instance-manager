using System;
using AvaloniaApplication1.Models;

namespace AvaloniaApplication1.Resolvers;

public class DisplaySelectionMapper
{
    public static DisplaySelection Map(DisplayOption option) =>
        option switch
        {
            DisplayOption.Primary => new DisplaySelection.Primary(),
            DisplayOption.Specific s => new DisplaySelection.Specific(s.Id),
            _ => throw new InvalidOperationException($"Unexpected display option: {option.GetType().Name}")
        };
}