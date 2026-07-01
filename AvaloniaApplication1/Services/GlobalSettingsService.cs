using System;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Snapshots;

namespace AvaloniaApplication1.Services;

public class GlobalSettingsService(ConfigService configService)
{
    public async Task SaveAsync(GlobalSettingsDraft draft)
    {
        // Validate draft

        var snapshot = GetSnapshot() with
        {
            GameExecutablePath = draft.GameExecutablePath,
            CenterMouseCursorInRecalledWindow = draft.CenterMouseCursorInRecalledWindow,
            FallbackToPrimaryDisplayIfInvalid = draft.FallbackToPrimaryDisplayIfInvalid,
            CloseInstancesOnAppExit = draft.CloseInstancesOnAppExit
        };
        
        await configService.ChangeAsync(context => context.UpdateGlobalSettings(snapshot));   

    }
    
    public GlobalSettingsSnapshot GetSnapshot()
    {
        return configService.Config.GetGlobalSettings();
    }
}