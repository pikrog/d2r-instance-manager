using System.Text.Json.Serialization;

namespace AvaloniaApplication1.Secret.Options.Encryption;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(NoEncryption), "noEncryption")]
[JsonDerivedType(typeof(DpapiCurrentUserEncryption), "dpapiCurrentUser")]
[JsonDerivedType(typeof(AesGcmEncryption), "aesGcm")]
public abstract record EncryptionOptions;