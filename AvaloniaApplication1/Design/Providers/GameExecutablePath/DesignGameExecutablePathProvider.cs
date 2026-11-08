using D2RInstanceManager.GameExecutable.Providers;

namespace D2RInstanceManager.Design.Providers.GameExecutablePath;

public class DesignGameExecutablePathProvider : IGameExecutablePathProvider
{
    public string TryGet()
    {
        return @"C:\Program Files (x86)\Diablo II Resurrected\D2R.exe";
    }
}