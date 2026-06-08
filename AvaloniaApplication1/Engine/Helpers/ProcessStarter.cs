using System;
using System.IO;
using AvaloniaApplication1.Engine.Exceptions;
using AvaloniaApplication1.Engine.Models.Common;
using AvaloniaApplication1.Engine.Models.Platform.Process;
using AvaloniaApplication1.Engine.Platform;

namespace AvaloniaApplication1.Engine.Helpers;

public static class ProcessStarter
{
    public enum FailureReason
    {
        FileNotFound,
        AccessDenied,
    }

    public abstract record StartResult
    {
        private StartResult() { }
        
        public sealed record Success(Process Process) : StartResult;

        public sealed record Failure(FailureReason Reason) : StartResult;
    }
    
    public static StartResult Start(ProcessStartInfo startInfo)
    {
        try
        {
            var process = Process.Start(startInfo);
            return new StartResult.Success(process);
        }
        catch (FileNotFoundException e)
        {
            return new StartResult.Failure(FailureReason.FileNotFound);
        }
        catch (UnauthorizedAccessException e)
        {
            return new StartResult.Failure(FailureReason.AccessDenied);
        }
    }
}