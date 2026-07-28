using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace AvaloniaApplication1.GameExecutable;

public static partial class GameExecutableFileValidator
{
    internal enum BinaryType : uint
    {
        Executable64Bit = 0x6,
    }
    
    [LibraryImport("kernel32.dll", EntryPoint = "GetBinaryTypeW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetBinaryType(string applicationName, out BinaryType binaryType);
    
    private const uint BadExeFormat = 0xC1;

    public enum Result
    {
        Ok,
        MissingPath,
        FileNotFound,
        InvalidExecutableFormat,
        UnrecognizedExecutable,
    }

    public static Result Validate(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return Result.MissingPath;
        
        if (!File.Exists(path))
            return Result.FileNotFound;

        if (!GetBinaryType(path, out var type))
        {
            return Marshal.GetLastPInvokeError() == BadExeFormat 
                ? Result.InvalidExecutableFormat 
                : Result.FileNotFound;
        }

        if (type != BinaryType.Executable64Bit)
            return Result.InvalidExecutableFormat;
        
        var fileVersionInfo = FileVersionInfo.GetVersionInfo(path);
        return fileVersionInfo.ProductName != GameExecutableConstants.ProductName 
            ? Result.UnrecognizedExecutable 
            : Result.Ok;
    }
}