using AvaloniaApplication1.Common;

namespace AvaloniaApplication1.Config.Exceptions;

public class ConfigException(string message) : ApplicationException(message);