namespace AvaloniaApplication1.Engine.Exceptions.Platform;

public class ProcessStartAccessDeniedException(string path) : ProcessException($"Access denied to file: {path}");