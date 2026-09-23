using System.CommandLine;

namespace AvaloniaApplication1.CommandLine;

public static class CliParser
{
    public static CliOptions Parse(string[] args)
    {
        var logLevelOption = new Option<CliLogLevel>(
            name: "--log-level",
            description: "Sets the application log level",
            getDefaultValue: () => CliLogLevel.Information
            )
        {
            IsRequired = false
        };

        var rootCommand = new RootCommand("Avalonia Application CLI");
        rootCommand.AddOption(logLevelOption);

        var parseResult = rootCommand.Parse(args);

        var options = new CliOptions
        {
            CliLogLevel = parseResult.GetValueForOption(logLevelOption)
        };

        return options;
    }
}
