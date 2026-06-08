using System;

namespace AvaloniaApplication1.Engine.Exceptions.Platform;

public class ProcessUnknownError(Exception exception) : ProcessException(exception.Message, exception);