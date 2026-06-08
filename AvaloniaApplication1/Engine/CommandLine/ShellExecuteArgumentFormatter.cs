namespace AvaloniaApplication1.Engine.CommandLine;

public class ShellExecuteArgumentFormatter(string argumentNamePrefix = "-", string argumentNameValueSeparator = " ") : IArgumentFormatter
{
    public string ArgumentNamePrefix { get; } = argumentNamePrefix;

    public string ArgumentNameValueSeparator { get; } = argumentNameValueSeparator;

    private static string Quote(string s) => $"\"{s}\"";

    private static string Escape(string s) => s.Replace("\"", "\"\"");

    private string ArgumentName(string name) => $"{ArgumentNamePrefix}{name}";

    private static string ArgumentValue(string value) => Quote(Escape(value));

    public string Parameter(string name, string value)
        => string.Join(
            ArgumentNameValueSeparator,
            [ArgumentName(name), ArgumentValue(value)]
        );

    public string Flag(string name) => $"{ArgumentNamePrefix}{name}";

}