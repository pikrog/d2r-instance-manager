namespace AvaloniaApplication1.Engine.Exceptions.Platform;

public class ProcessStillActiveException(uint id) : ProcessException($"Process with id {id} is still active.");