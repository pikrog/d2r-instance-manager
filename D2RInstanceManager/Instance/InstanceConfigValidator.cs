using System.Collections.Generic;
using D2RInstanceManager.Config;
using D2RInstanceManager.Display;
using D2RInstanceManager.Instance.Models;
using D2RInstanceManager.Instance.Models.Issues;

namespace D2RInstanceManager.Instance;

public class InstanceConfigValidator(ConfigService configService)
{
    public IReadOnlyList<InstanceIssue> Validate(InstanceSnapshot snapshot)
    {
        var issues = new List<InstanceIssue>();
        
        // todo: persistence validation

        if (snapshot.Display is DisplaySelection.Specific display && DisplayResolver.ResolveDisplayId(display, false) is null)
        {
            var isFallbackAllowed = configService.Config.GetGlobalSettings().FallbackToPrimaryDisplayIfInvalid;

            if (!configService.Config.DisplayExists(display.Id))
            {
                issues.Add(new InvalidDisplay());
            }
            else
            {
                var cachedDisplay = configService.Config.GetDisplay(display.Id);
                var missingDisplay = new MissingDisplay(cachedDisplay, isFallbackAllowed);
                issues.Add(missingDisplay);
            }
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