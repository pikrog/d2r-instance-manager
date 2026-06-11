using AvaloniaApplication1.Config;
using AvaloniaApplication1.Engine.Providers;

namespace AvaloniaApplication1.Services;

public class DisplayResolverSettingsProvider(ConfigService configService) : IDisplayResolverSettingsProvider
{
    public bool AllowFallbackToPrimaryDisplay => 
        configService.Config.GetGlobalSettings().FallbackToPrimaryDisplayIfInvalid;
}