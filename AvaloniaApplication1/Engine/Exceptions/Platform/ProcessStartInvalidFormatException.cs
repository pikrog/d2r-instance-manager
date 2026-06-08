namespace AvaloniaApplication1.Engine.Exceptions.Platform;

public class ProcessStartInvalidFormatException(string path) : ProcessException($"Invalid file format: {path}");