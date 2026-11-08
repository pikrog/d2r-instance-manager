using System.Collections.Generic;

namespace D2RInstanceManager.Authentication.Models;

public record AuthenticationMethodOption(AuthenticationMethod AuthenticationMethod, string Label)
{
    public static readonly AuthenticationMethodOption OsiTokenRegistry = 
        new(AuthenticationMethod.OsiTokenRegistry, "OSI Token Registry");
    
    public static readonly AuthenticationMethodOption CommandLineArguments = 
        new(AuthenticationMethod.CommandLineArguments, "Command Line Arguments");

    public static readonly IReadOnlyList<AuthenticationMethodOption> AllOptions =
        [OsiTokenRegistry, CommandLineArguments];
}
