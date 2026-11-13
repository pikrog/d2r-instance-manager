using System;

namespace D2RInstanceManager.Error;

public abstract record ErrorDescriptor(Exception Exception)
{
    public sealed record StartupFailure(Exception Exception) : ErrorDescriptor(Exception);

    public sealed record RuntimeFailure(Exception Exception) : ErrorDescriptor(Exception);

    public sealed record ShutdownFailure(Exception Exception) : ErrorDescriptor(Exception);
};