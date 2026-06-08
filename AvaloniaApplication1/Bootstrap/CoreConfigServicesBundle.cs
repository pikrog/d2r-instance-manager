using AvaloniaApplication1.Config;

namespace AvaloniaApplication1.Bootstrap;

public record CoreConfigServicesBundle(
    AppEnvironment AppEnvironment,
    IConfigStore ConfigStore,
    ConfigLoader ConfigLoader,
    AppConfig AppConfig
    );