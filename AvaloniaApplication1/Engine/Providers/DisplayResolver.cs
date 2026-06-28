using System;
using AvaloniaApplication1.Engine.Platform;

namespace AvaloniaApplication1.Engine.Providers;

public static class DisplayResolver
{
    public static Display? GetById(string id) => Display.GetById(id);
    
    /*public static Display GetByIndex(int index, bool allowFallbackToPrimary = false)
    {
        try
        {
            return Display.GetByIndex(index);
        }
        catch (ArgumentOutOfRangeException) // todo: replace exception with Result<Display, Error>.
        {
            if (!allowFallbackToPrimary)
                throw;
            return Display.GetPrimary();
        }
    }*/
}