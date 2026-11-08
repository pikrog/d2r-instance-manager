using System.Threading.Tasks;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Secret;

namespace AvaloniaApplication1.GlobalSettings;

public class GlobalSettingsService(ConfigService configService, ISecretMigrationCoordinator secretMigrationCoordinator)
{
    public async Task SaveAsync(GlobalSettingsDraft draft)
    {
        await configService.ChangeAsync(context =>
        {
            var snapshot = GetSnapshot() with
            {
                GameExecutablePath = draft.GameExecutablePath,
                CenterMouseCursorInShownWindow = draft.CenterMouseCursorInShownWindow,
                FallbackToPrimaryDisplayIfInvalid = draft.FallbackToPrimaryDisplayIfInvalid,
                CloseInstancesOnAppExit = draft.CloseInstancesOnAppExit
            };
            
            context.UpdateGlobalSettings(snapshot);
        });
    }

    public Task SetProtectionSettingsAsync(ProtectionSelection selection, string? passphrase)
    {
        var request = EncryptionRequestMapper.Map(selection, passphrase);
        return secretMigrationCoordinator.MigrateAsync(request);
    }
    
    public GlobalSettingsSnapshot GetSnapshot()
    {
        return configService.Config.GetGlobalSettings();
    }
}