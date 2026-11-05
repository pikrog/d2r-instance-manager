namespace AvaloniaApplication1.Secret.Options.Protector;

public sealed record DpapiCurrentUserProtectorOptions : ProtectorOptions
{
    public static readonly DpapiCurrentUserProtectorOptions Instance = new();
}