namespace AvaloniaApplication1.Engine.Platform.Exceptions;

internal class NtException : PlatformException
{
    public WinApi.NtStatus Status { get; }
            
    public NtException(string message) : base(message) { }

    public NtException(WinApi.NtStatus status) : base($"NT API call failed with status 0x{status:X8} ({status})")
    {
        Status = status;
    }
}