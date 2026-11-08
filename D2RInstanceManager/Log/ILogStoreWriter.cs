namespace D2RInstanceManager.Log;

public interface ILogStoreWriter
{
    void Append(LogEntry logEntry);
}