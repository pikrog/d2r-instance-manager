namespace AvaloniaApplication1.Secret.Requests;

public sealed record DpapiCurrentUserRequest : EncryptionRequest
{
    public static readonly DpapiCurrentUserRequest Instance = new();
}