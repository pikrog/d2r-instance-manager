using System;

namespace AvaloniaApplication1.Common;

public abstract class ApplicationException(string message) : Exception(message);