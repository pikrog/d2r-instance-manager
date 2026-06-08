namespace AvaloniaApplication1.Engine.Exceptions.Platform;

public class ProcessAlreadyExitedException(uint id) : ProcessException($"Process with id {id} already exited.");