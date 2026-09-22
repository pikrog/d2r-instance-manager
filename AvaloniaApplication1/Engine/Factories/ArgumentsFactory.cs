using System;
using System.Collections.Generic;
using AvaloniaApplication1.Engine.CommandLine;
using AvaloniaApplication1.Engine.Models.Contexts.Arguments;

namespace AvaloniaApplication1.Engine.Factories;

public static class ArgumentsFactory
{
    private const string UsernameParam = "username";
    private const string PasswordParam = "password";
    private const string AddressParam = "address";
    private const string UidParam = "uid";
    private const string UidOsiValue = "osi";

    private const string NoSoundFlag = "ns";
    private const string WindowedModeFlag = "w";
    
    public static List<Argument> Create(ArgumentsContext context)
    {
        List<Argument> arguments = [];

        switch (context.AuthenticationArgumentsContext)
        {
            case CliArgumentsContext c:
                arguments.AddRange([
                    new ParametrizedArgument(UsernameParam, c.AccountUsername),
                    new SensitiveParametrizedArgument(PasswordParam, c.AccountPassword),
                    new ParametrizedArgument(AddressParam, c.RegionAddress)
                ]);
                break;
            case OsiArgumentsContext c:
                arguments.AddRange([
                    new ParametrizedArgument(UidParam, UidOsiValue),
                    new ParametrizedArgument(AddressParam, c.RegionAddress),
                ]);
                break;
            case OfflineArgumentsContext:
                break;
            default:
                throw new InvalidOperationException($"Unexpected authentication arguments context: {context.AuthenticationArgumentsContext.GetType().Name}");
        }

        if (context.IsNoSound)
            arguments.Add(new FlagArgument(NoSoundFlag));
        if (context.IsWindowedMode)
            arguments.Add(new FlagArgument(WindowedModeFlag));
        
        return arguments;
    }
}