using AvaloniaApplication1.Providers.GameExecutablePath;

namespace AvaloniaApplication1.Design.Providers.GameExecutablePath;

public class DesignGameExecutablePathProvider : IGameExecutablePathProvider
{
    public string TryGet()
    {
        return @"C:\Program Files (x86)\Diablo II Resurrected\D2R.exe";
    }
}