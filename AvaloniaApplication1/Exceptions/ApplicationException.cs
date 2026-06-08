using System;

namespace AvaloniaApplication1.Exceptions;

public abstract class ApplicationException(string message) : Exception(message);