using System;
using AvaloniaApplication1.Engine.Models.Contexts;
using AvaloniaApplication1.Engine.Models.Contexts.Arguments;
using AvaloniaApplication1.Engine.Models.Contexts.Launch;

namespace AvaloniaApplication1.Engine.Mappers;

public static class ArgumentsContextMapper
{
    public static ArgumentsContext Map(LaunchContext context)
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

        return new ArgumentsContext()
        {
            AuthenticationArgumentsContext = authenticationArgumentsContext,
            IsNoSound = context.IsNoSound,
            IsWindowedMode = context.IsWindowedMode
        };
    }
}