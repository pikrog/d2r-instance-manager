using System;
using System.IO;
using AvaloniaApplication1.Engine.Exceptions;
using AvaloniaApplication1.Engine.Exceptions.Platform;
using AvaloniaApplication1.Engine.Models.Common;
using AvaloniaApplication1.Engine.Models.Platform.Process;
using AvaloniaApplication1.Engine.Platform;

namespace AvaloniaApplication1.Engine.Helpers;

public static class ProcessStarter
{
    public abstract record StartResult
    {
        private StartResult() { }
        
        public sealed record Success(Process Process) : StartResult;

        public sealed record Failure(ProcessStartException Exception) : StartResult;
    }
    
    public static StartResult Start(ProcessStartInfo startInfo)
    {
        try
        {
            var process = Process.Start(startInfo);
            return new StartResult.Success(process);
        }
        catch (ProcessStartException e)
        {
            return new StartResult.Failure(e);
        }
    }
}