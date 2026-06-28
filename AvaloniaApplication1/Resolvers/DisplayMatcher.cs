using AvaloniaApplication1.Models;

namespace AvaloniaApplication1.Mappers;

public static class DisplayMatcher
{
    public static bool IsMatch(DisplaySelection selection, DisplayOption option) =>
        selection switch
        {
            DisplaySelection.Primary => option is DisplayOption.Primary,
            DisplaySelection.Specific s => option is DisplayOption.Specific o && o.Id == s.Id,
            _ => false
        };
}