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

    public enum ValidateResultCode
    {
        Ok,
        MissingPath,
        FileNotFound,
        InvalidExecutableFormat,
        UnrecognizedExecutable,
    }

    public readonly record struct ValidateResult(ValidateResultCode Code, FileMetadata? FileMetadata = null);

    public static ValidateResult Validate(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return new ValidateResult(ValidateResultCode.MissingPath);
        
        if (!File.Exists(path))
            return new ValidateResult(ValidateResultCode.FileNotFound);

        var fileVersionInfo = FileVersionInfo.GetVersionInfo(path);
        var fileMetadata = new FileMetadata(
            fileVersionInfo.ProductName,
            fileVersionInfo.CompanyName,
            fileVersionInfo.FileDescription,
            fileVersionInfo.FileVersion
        );
        
        if (!GetBinaryType(path, out var type))
        {
            return Marshal.GetLastPInvokeError() == BadExeFormat
                ? new ValidateResult(ValidateResultCode.InvalidExecutableFormat, fileMetadata)
                : new ValidateResult(ValidateResultCode.FileNotFound);
        }

        if (type != BinaryType.Executable64Bit)
            return new ValidateResult(ValidateResultCode.InvalidExecutableFormat, fileMetadata);
        
        return fileVersionInfo.ProductName != GameExecutableConstants.ProductName 
            ? new ValidateResult(ValidateResultCode.UnrecognizedExecutable, fileMetadata)
            : new ValidateResult(ValidateResultCode.Ok, fileMetadata);
    }
}