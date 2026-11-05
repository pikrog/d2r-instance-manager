using System;
using System.Security.Cryptography;
using AvaloniaApplication1.Secret.Data;

namespace AvaloniaApplication1.Secret.Protectors;

public sealed class AesGcmSecretProtector(byte[] key) : ISecretProtector
{
    private const int NonceSize = 12;
    private const int TagSize = 16;
    
    public SecretData Protect(byte[] data)
    {
        var nonce = RandomNumberGenerator.GetBytes(NonceSize);
        var ciphertext = new byte[data.Length];
        var tag = new byte[TagSize];
        
        using var aes = new AesGcm(key, TagSize);
        
        aes.Encrypt(nonce, data, ciphertext, tag);

        return new AesSecretData(ciphertext, nonce, tag);
    }

    public byte[] Unprotect(SecretData data)
    {
        if (data is not AesSecretData aesData)
            throw new InvalidOperationException("Invalid secret data type. Expected AesSecretData");
        
        var plaintext = new byte[data.Payload.Length];
        
        using var aes = new AesGcm(key, TagSize);
        
        aes.Decrypt(aesData.Nonce, aesData.Payload, aesData.Tag, plaintext);
        
        return plaintext;
    }
}