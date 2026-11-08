using System;
using D2RInstanceManager.Secret.Options.Encryption;

namespace D2RInstanceManager.GlobalSettings;

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