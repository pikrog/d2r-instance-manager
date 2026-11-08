namespace D2RInstanceManager.Secret.Requests;

public sealed record NoEncryptionRequest : EncryptionRequest
{
    public static readonly NoEncryptionRequest Instance = new();
}