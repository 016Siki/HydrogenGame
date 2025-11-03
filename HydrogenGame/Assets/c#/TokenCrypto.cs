// Assets/c#/TokenCrypto.cs
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class TokenCrypto
{
    private const string Passphrase = "SuisokaApp_TokenKey_2025-08-27_A2!vX7r$BqP9@zH5kM1";
    private static readonly byte[] Salt = Encoding.UTF8.GetBytes("Suisoka_Salt_v1.2.3_2025");

    private const int Iterations = 10000; // PBKDF2反復回数（必要に応じて上げる）

    private static byte[] DeriveKey32()
    {
        using var kdf = new Rfc2898DeriveBytes(Passphrase, Salt, Iterations, HashAlgorithmName.SHA256);
        return kdf.GetBytes(32); // 32byte = AES-256
    }

    public static string EncryptAES(string plain)
    {
        using var aes = Aes.Create();         
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = DeriveKey32();
        aes.GenerateIV();                    

        using var ms = new MemoryStream();
        // 先頭にIVを書き込む（復号時に取り出す）
        ms.Write(aes.IV, 0, aes.IV.Length);

        using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs, Encoding.UTF8))
        {
            sw.Write(plain);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public static string DecryptAES(string base64)
    {
        var data = Convert.FromBase64String(base64);
        if (data.Length < 16) throw new ArgumentException("Invalid payload");

        var iv = new byte[16];
        Buffer.BlockCopy(data, 0, iv, 0, 16);

        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = DeriveKey32();
        aes.IV = iv;

        using var ms = new MemoryStream(data, 16, data.Length - 16);
        using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using var sr = new StreamReader(cs, Encoding.UTF8);
        return sr.ReadToEnd();
    }
}
