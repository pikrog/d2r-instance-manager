namespace AvaloniaApplication1.Engine.Exceptions.Platform;

public class ProcessNotFoundException(uint id) : ProcessException($"Process with id {id} not found.");