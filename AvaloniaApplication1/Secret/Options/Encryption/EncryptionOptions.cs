using System.Text.Json.Serialization;

namespace D2RInstanceManager.Secret.Options.Encryption;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(NoEncryption), "noEncryption")]
[JsonDerivedType(typeof(DpapiCurrentUserEncryption), "dpapiCurrentUser")]
[JsonDerivedType(typeof(AesGcmEncryption), "aesGcm")]
public abstract record EncryptionOptions;