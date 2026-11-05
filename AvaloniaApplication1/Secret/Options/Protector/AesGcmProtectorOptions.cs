namespace AvaloniaApplication1.Secret.Options.Protector;

public sealed record AesGcmProtectorOptions(MasterKey Key) : ProtectorOptions;