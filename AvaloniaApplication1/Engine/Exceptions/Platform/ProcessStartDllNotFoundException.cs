namespace AvaloniaApplication1.Engine.Exceptions.Platform;

public class ProcessStartDllNotFoundException(string path) : ProcessException($"Dll not found: {path}");