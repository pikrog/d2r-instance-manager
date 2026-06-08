namespace AvaloniaApplication1.Engine.Exceptions.Platform;

public class ProcessStartFileNotFoundException(string path) : ProcessException($"File not found: {path}");