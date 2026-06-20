using System;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Snapshots;

namespace AvaloniaApplication1.Services;

public class GlobalSettingsService(ConfigService configService)
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    
    public async Task Save(GlobalSettingsDraft draft)
    {
        // Validate draft

        var snapshot = GetSnapshot() with
        {
            GameExecutablePath = draft.GameExecutablePath,
            CenterMouseCursorInRecalledWindow = draft.CenterMouseCursorInRecalledWindow,
            FallbackToPrimaryDisplayIfInvalid = draft.FallbackToPrimaryDisplayIfInvalid,
            CloseInstancesOnAppExit = draft.CloseInstancesOnAppExit
        };
        
        await _semaphore.WaitAsync();
        try
        {
            await configService.ChangeAsync(context => context.UpdateGlobalSettings(snapshot));   
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    public GlobalSettingsSnapshot GetSnapshot()
    {
        return configService.Config.GetGlobalSettings();
    }
}