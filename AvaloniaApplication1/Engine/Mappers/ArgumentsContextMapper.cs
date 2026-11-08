using System;
using D2RInstanceManager.Engine.Models.Contexts.Arguments;
using D2RInstanceManager.Engine.Models.Contexts.Launch;

namespace D2RInstanceManager.Engine.Mappers;

public static class ArgumentsContextMapper
{
    public static ArgumentsContext Map(InstanceLaunchContext context)
    {
        AuthenticationArgumentsContext authenticationArgumentsContext = context.AuthenticationContext switch
        {
            CliAuthenticationContext c => new CliArgumentsContext(
                c.AccountUsername,
                c.AccountPassword,
                c.RegionAddress),
            OfflineAuthenticationContext c => new OfflineArgumentsContext(),
            OsiAuthenticationContext c => new OsiArgumentsContext(c.RegionAddress),
            _ => throw new InvalidOperationException($"Unexpected authentication context: {context.AuthenticationContext.GetType().Name}")
        };

        return new ArgumentsContext
        {
            AuthenticationArgumentsContext = authenticationArgumentsContext,
            IsNoSound = context.IsNoSound,
            IsWindowedMode = context.IsWindowedMode
        };
    }
}