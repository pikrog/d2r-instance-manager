using System;

namespace AvaloniaApplication1.Engine.Exceptions.Platform;

public class ProcessAccessDeniedException: ProcessException
{
    public ProcessAccessDeniedException(uint id) : base($"Access denied to process with id {id}.") { }
    
    public ProcessAccessDeniedException(IntPtr handle) : base($"Access denied to process with handle 0x{handle:X8}.") { }
}