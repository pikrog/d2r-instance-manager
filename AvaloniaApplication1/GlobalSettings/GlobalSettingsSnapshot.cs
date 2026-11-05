using AvaloniaApplication1.Authentication.Models;
using AvaloniaApplication1.Secret;

namespace AvaloniaApplication1.GlobalSettings;

public record GlobalSettingsSnapshot
{
    public string GameExecutablePath { get; init; } = "";
    public int UnlockMultiboxMaxRetries { get; init; } = 10;
    public int UnlockMultiboxRetryDelayMs { get; init; } = 500;
    public int GracefulInstanceCloseTimeoutMs { get; init; } = 250;
    public int GracefulInstanceCloseRetries { get; init; } = 20;
    public int ForcefulInstanceCloseTimeoutMs { get; init; } = 3000;
    public bool CenterMouseCursorInShownWindow { get; init; } = true;
    public bool FallbackToPrimaryDisplayIfInvalid { get; init; } = true;
    public bool CloseInstancesOnAppExit { get; init; } = true;
    public AuthenticationMethod AuthenticationMethod { get; init; } = AuthenticationMethod.OsiTokenRegistry;
    public SecretProtectionSettings SecretProtection { get; init; } = new();
}