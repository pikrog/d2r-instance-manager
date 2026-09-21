using System;
using AvaloniaApplication1.Engine.Platform;

namespace AvaloniaApplication1.Display;

public static class DisplayResolver
{
    public static ResolvedDisplay? ResolveDisplayId(DisplaySelection selection, bool isFallbackAllowed) =>
        selection switch
        {
            DisplaySelection.Primary => 
                new ResolvedDisplay(DisplayInfo.GetPrimary().Id, false),
            
            DisplaySelection.Specific specific => 
                DisplayInfo.Exists(specific.Id) 
                    ? new ResolvedDisplay(specific.Id, false)
                    : isFallbackAllowed 
                        ? new ResolvedDisplay(DisplayInfo.GetPrimary().Id, true) 
                        : null,
            
            _ => throw new InvalidOperationException($"Unexpected display selection: {selection.GetType().Name}")
        };
}