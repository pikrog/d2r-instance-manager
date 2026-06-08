using System;
using System.Collections.Generic;
using AvaloniaApplication1.Engine.CommandLine;
using AvaloniaApplication1.Engine.Models.Contexts;
using AvaloniaApplication1.Engine.Models.Contexts.Arguments;
using AvaloniaApplication1.Engine.Models.Contexts.Launch;

namespace AvaloniaApplication1.Engine.Factories;

public class ArgumentsFactory(IArgumentStringBuilder argumentStringBuilder)
{
    private const string UsernameParam = "username";
    private const string PasswordParam = "password";
    private const string AddressParam = "address";
    private const string UidParam = "uid";
    private const string UidOsiValue = "osi";

    private const string NoSoundFlag = "ns";
    private const string WindowedModeFlag = "w";
    
    public static List<Argument> CreateList(ArgumentsContext context)
    {
        List<Argument> arguments = [];

        switch (context.AuthenticationArgumentsContext)
        {
            case CliArgumentsContext c:
                arguments.AddRange([
                    Argument.Parameter(UsernameParam, c.AccountUsername),
                    Argument.Parameter(PasswordParam, c.AccountPassword),
                    Argument.Parameter(AddressParam, c.RegionAddress)
                ]);
                break;
            case OsiArgumentsContext c:
                arguments.AddRange([
                    Argument.Parameter(UidParam, UidOsiValue),
                    Argument.Parameter(AddressParam, c.RegionAddress),
                ]);
                break;
            case OfflineArgumentsContext:
                break;
            default:
                throw new InvalidOperationException($"Unexpected authentication arguments context: {context.AuthenticationArgumentsContext.GetType().Name}");
        }

        if (context.IsNoSound)
            arguments.Add(Argument.Flag(NoSoundFlag));
        if (context.IsWindowedMode)
            arguments.Add(Argument.Flag(WindowedModeFlag));
        
        return arguments;
    }

    public string CreateString(ArgumentsContext context)
    {
        var list = CreateList(context);
        return argumentStringBuilder.Build(list);
    }
    
    /*public static List<Argument> CreateList(LaunchContext context) // todo: replace LaunchContext with LaunchArguments?
    {
        List<Argument> arguments = [];
        switch (context.AuthenticationContext)
        {
            case CliAuthenticationContext authContext:
                arguments.AddRange([
                    Argument.Parameter(UsernameParam, authContext.AccountUsername),
                    Argument.Parameter(PasswordParam, authContext.AccountPassword),
                    Argument.Parameter(AddressParam, authContext.RegionAddress),
                ]);
                break;
            case OsiAuthenticationContext authContext:
                arguments.Add(Argument.Parameter(AddressParam, authContext.RegionAddress));
                arguments.Add(Argument.Parameter(UidParam, UidOsiValue));
                break;
        }
        
        if (context.IsNoSound)
            arguments.Add(Argument.Flag(NoSoundFlag));
        if (context.IsWindowedMode)
            arguments.Add(Argument.Flag(WindowedModeFlag));
        return arguments;
    }
    
    public string CreateString(LaunchContext context)
    {
        var list = CreateList(context);
        return argumentStringBuilder.Build(list);
    }*/
}