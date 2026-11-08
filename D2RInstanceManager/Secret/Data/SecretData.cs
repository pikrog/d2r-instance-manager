using System.Text.Json.Serialization;

namespace D2RInstanceManager.Secret.Data;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(AesSecretData), "aes")]
public record SecretData(byte[] Payload);