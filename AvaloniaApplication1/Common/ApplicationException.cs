using System;

namespace D2RInstanceManager.Common;

public abstract class ApplicationException(string message) : Exception(message);