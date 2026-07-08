using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaApplication1.Config;

namespace AvaloniaApplication1.Display;

public class DisplayService(ConfigService configService)
{
    public async Task<IReadOnlyList<DisplayOption>> GetOptionsAsync(DisplaySelection.Specific? selection = null)
    {
        var displays = Engine.Platform.DisplayInfo.GetAll();
        
        await CacheAsync(displays);
        
        var options = displays
            .Select(DisplayOption (d) =>
                new DisplayOption.Specific(d.Index, d.Id, d.Description, d.Width, d.Height, true))
            .Prepend(new DisplayOption.Primary())
            .ToList();
        
        if (selection is null || options.Any(o => DisplayMatcher.IsMatch(selection, o))) 
            return options;
        
        var cachedDisplay = GetCached(selection.Id);
        var disconnectedOption = new DisplayOption.Specific(null, cachedDisplay.Id, cachedDisplay.Description, cachedDisplay.Width, cachedDisplay.Height, false);
        options.Add(disconnectedOption);
        return options;
    }
    
    public CachedDisplaySnapshot GetCached(string id) => configService.Config.GetCachedDisplay(id);

    private async Task CacheAsync(IReadOnlyList<Engine.Platform.DisplayInfo> displays)
    {
        await configService.ChangeAsync(c => 
            {
                foreach (var d in displays)
                {
                    var snapshot = new CachedDisplaySnapshot(d.Id, d.Description, d.Width, d.Height);
                    c.CacheDisplay(snapshot);
                }
            }
        );
    }

    private bool IsFallbackAllowed => configService.Config.GetGlobalSettings().FallbackToPrimaryDisplayIfInvalid;

    public string? ResolveDisplayId(DisplaySelection selection) =>
        selection switch
        {
            DisplaySelection.Primary => Engine.Platform.DisplayInfo.GetPrimary().Id,
            DisplaySelection.Specific specific => 
                Engine.Platform.DisplayInfo.Exists(specific.Id) 
                    ? specific.Id 
                    : IsFallbackAllowed ? Engine.Platform.DisplayInfo.GetPrimary().Id : null,
            _ => throw new InvalidOperationException($"Unexpected display selection: {selection.GetType().Name}")
        };
}