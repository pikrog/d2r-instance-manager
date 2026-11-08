using System;
using System.Security.Cryptography;

namespace D2RInstanceManager.Secret;

public sealed class MasterKey(byte[] key)
{
    public ReadOnlySpan<byte> Bytes => key;
    
    public void Clear() => CryptographicOperations.ZeroMemory(key);
}