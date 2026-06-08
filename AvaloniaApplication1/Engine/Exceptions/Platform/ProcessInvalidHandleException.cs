using System;

namespace AvaloniaApplication1.Engine.Exceptions.Platform;

public class ProcessInvalidHandleException : ProcessException
{
    public ProcessInvalidHandleException(uint id) : base($"Process {id} handle is invalid.") { }
    
    public ProcessInvalidHandleException(IntPtr handle) : base($"Process handle 0x{handle:X8} is invalid.") { }
}