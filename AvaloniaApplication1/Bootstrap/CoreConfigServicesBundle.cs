using D2RInstanceManager.Config;
using D2RInstanceManager.Config.Stores;

namespace D2RInstanceManager.Bootstrap;

public record CoreConfigServicesBundle(
    ConfigEnvironment ConfigEnvironment,
    IConfigStore ConfigStore,
    ConfigLoader ConfigLoader,
    AppConfig AppConfig
    );