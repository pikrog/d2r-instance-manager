using System;
using D2RInstanceManager.Secret.Requests;

namespace D2RInstanceManager.GlobalSettings;

public static class EncryptionRequestMapper
{
    public static EncryptionRequest Map(ProtectionSelection selection, string? passphrase) => selection switch
    {
        ProtectionSelection.Disabled => NoEncryptionRequest.Instance,
        
        ProtectionSelection.ProtectedByWindowsAccount => DpapiCurrentUserRequest.Instance,
        
        ProtectionSelection.ProtectedByPassphrase => passphrase is not null 
            ? new AesGcmRequest(passphrase) 
            : throw new InvalidOperationException("Passphrase is required for AES-GCM encryption"), 
        
        _ => throw new ArgumentOutOfRangeException(nameof(selection))
    };
}