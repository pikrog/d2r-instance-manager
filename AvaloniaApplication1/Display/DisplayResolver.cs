using System;

namespace AvaloniaApplication1.Display;

public static class DisplayResolver
{
    public static string? ResolveDisplayId(DisplaySelection selection, bool isFallbackAllowed) =>
        selection switch
        {
            DisplaySelection.Primary => Engine.Platform.DisplayInfo.GetPrimary().Id,
            DisplaySelection.Specific specific => 
                Engine.Platform.DisplayInfo.Exists(specific.Id) 
                    ? specific.Id 
                    : isFallbackAllowed ? Engine.Platform.DisplayInfo.GetPrimary().Id : null,
            _ => throw new InvalidOperationException($"Unexpected display selection: {selection.GetType().Name}")
        };
}