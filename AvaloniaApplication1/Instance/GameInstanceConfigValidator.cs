using System.Collections.Generic;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Display;
using AvaloniaApplication1.Instance.Models;
using AvaloniaApplication1.Instance.Models.Issues;

namespace AvaloniaApplication1.Instance;

public class GameInstanceConfigValidator(ConfigService configService)
{
    public IReadOnlyList<GameInstanceIssue> Validate(GameInstanceSnapshot snapshot)
    {
        var issues = new List<GameInstanceIssue>();
        
        // todo: persistence validation

        if (snapshot.Display is DisplaySelection.Specific display && DisplayResolver.ResolveDisplayId(display, false) is null)
        {
            var isFallbackAllowed = configService.Config.GetGlobalSettings().FallbackToPrimaryDisplayIfInvalid;
            var cachedDisplay = configService.Config.GetCachedDisplay(display.Id);
            var missingDisplay = new MissingDisplay(cachedDisplay, isFallbackAllowed);
            issues.Add(missingDisplay);
        }

        if (snapshot.IsOnlineMode)
        {
            if (snapshot.RegionId is null)
                issues.Add(new MissingRegion());
            else if(!configService.Config.RegionExists(snapshot.RegionId.Value))
                issues.Add(new DeletedRegion());

            if (snapshot.AccountId is null)
                issues.Add(new MissingAccount());
            else if (!configService.Config.AccountExists(snapshot.AccountId.Value))
                issues.Add(new DeletedAccount());
        }

        return issues;
    }
}