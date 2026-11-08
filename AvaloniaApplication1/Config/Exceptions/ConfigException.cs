using D2RInstanceManager.Common;

namespace D2RInstanceManager.Config.Exceptions;

public class ConfigException(string message) : ApplicationException(message);