using AvaloniaApplication1.Config;
using AvaloniaApplication1.Config.Stores;

namespace AvaloniaApplication1.Bootstrap;

public record CoreConfigServicesBundle(
    ConfigEnvironment ConfigEnvironment,
    IConfigStore ConfigStore,
    ConfigLoader ConfigLoader,
    AppConfig AppConfig
    );