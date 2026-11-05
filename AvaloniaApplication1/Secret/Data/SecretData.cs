using System.Text.Json.Serialization;

namespace AvaloniaApplication1.Secret.Data;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(AesSecretData), "aes")]
public record SecretData(byte[] Payload);