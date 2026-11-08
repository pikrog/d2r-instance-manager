namespace D2RInstanceManager.Secret.Options.Protector;

public sealed record AesGcmProtectorOptions(MasterKey Key) : ProtectorOptions;