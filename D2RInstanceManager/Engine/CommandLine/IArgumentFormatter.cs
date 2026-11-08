namespace D2RInstanceManager.Engine.CommandLine;

public interface IArgumentFormatter
{
    string Parameter(string name, string value);
    string Flag(string name);
}