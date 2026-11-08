using System.Collections.Generic;
using System.Linq;
using D2RInstanceManager.Config;

namespace D2RInstanceManager.Display;

public class DisplayService(ConfigService configService)
{
    public IReadOnlyList<DisplayOption> GetOptions(DisplaySelection.Specific? selection = null)
    {
        var displays = Engine.Platform.DisplayInfo.GetAll();
        
        var options = displays
            .Select(DisplayOption (d) =>
                new DisplayOption.Specific(d.Index, 
                    new DisplaySnapshot(d.Id, d.Description, d.Width, d.Height), 
                    true))
            .Prepend(DisplayOption.Primary.Instance)
            .ToList();
        
        if (selection is null || options.Any(o => DisplayMatcher.IsMatch(selection, o))) 
            return options;
        
        var cachedDisplay = configService.Config.GetDisplay(selection.Id);
        var disconnectedOption = new DisplayOption.Specific(null, cachedDisplay, false);
        options.Add(disconnectedOption);
        return options;
    }
}