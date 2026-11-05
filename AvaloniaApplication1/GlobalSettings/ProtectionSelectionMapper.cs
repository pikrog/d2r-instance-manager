using System;
using AvaloniaApplication1.Secret.Options.Encryption;

namespace AvaloniaApplication1.GlobalSettings;

public static class ProtectionSelectionMapper
{
    public static ProtectionSelection Map(EncryptionOptions options) => options switch
    {
        NoEncryption => ProtectionSelection.Disabled,
        DpapiCurrentUserEncryption => ProtectionSelection.ProtectedByWindowsAccount,
        AesGcmEncryption => ProtectionSelection.ProtectedByPassphrase,
        _ => throw new ArgumentOutOfRangeException(nameof(options))
    };
}