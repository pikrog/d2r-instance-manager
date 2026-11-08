using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace D2RInstanceManager.GameExecutable;

using ValidationResult = GameExecutableFileValidationResult;
using ValidationResultCode = GameExecutableFileValidationResultCode;

public partial class GameExecutableFileValidator : IGameExecutableFileValidator
{
    internal enum BinaryType : uint
    {
        Executable64Bit = 0x6,
    }
    
    [LibraryImport("kernel32.dll", EntryPoint = "GetBinaryTypeW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetBinaryType(string applicationName, out BinaryType binaryType);
    
    private const uint BadExeFormat = 0xC1;

    public ValidationResult Validate(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return new ValidationResult(ValidationResultCode.MissingPath);
        
        if (!File.Exists(path))
            return new ValidationResult(ValidationResultCode.FileNotFound);

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
                ? new ValidationResult(ValidationResultCode.InvalidExecutableFormat, fileMetadata)
                : new ValidationResult(ValidationResultCode.FileNotFound);
        }

        if (type != BinaryType.Executable64Bit)
            return new ValidationResult(ValidationResultCode.InvalidExecutableFormat, fileMetadata);
        
        return fileVersionInfo.ProductName != GameExecutableConstants.ProductName 
            ? new ValidationResult(ValidationResultCode.UnrecognizedExecutable, fileMetadata)
            : new ValidationResult(ValidationResultCode.Ok, fileMetadata);
    }
}