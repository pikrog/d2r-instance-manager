using System;
using AvaloniaApplication1.Engine.Platform;

namespace AvaloniaApplication1.Engine.Providers;

public class DisplayResolver(IDisplayResolverSettingsProvider settingsProvider)
{
    public Display GetByIndex(int index)
    {
        try
        {
            return Display.GetByIndex(index);
        }
        catch (ArgumentOutOfRangeException)
        {
            if (!settingsProvider.AllowFallbackToPrimaryDisplay)
                throw;
            return Display.GetPrimary();
        }
    }
}