namespace AvaloniaApplication1.Log;

public interface ILogStoreWriter
{
    void Append(LogEntry logEntry);
}