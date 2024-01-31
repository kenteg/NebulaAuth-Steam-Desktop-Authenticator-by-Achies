﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NebulaAuth.Model;

public static class PHandler //RETHINK: Use SecureString?
{
    public static bool IsPasswordSet => _k.Length > 0;
    private static byte[] _k = Array.Empty<byte>();


    public static void SetPassword(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            _k = Array.Empty<byte>();
            return;
        }

        var keyBytes = Encoding.UTF8.GetBytes(password);
        _k = SHA256.HashData(keyBytes);
    }

    public static string Encrypt(string plainText)
    {
        if (_k.Length == 0) throw new Exception("Password not set");
        byte[] encryptedBytes;

        using (var aes = Aes.Create())
        {
            aes.Key = _k;
            aes.IV = new byte[16];

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                }

                encryptedBytes = memoryStream.ToArray();
            }
        }

        return Convert.ToBase64String(encryptedBytes);
    }

    public static string Decrypt(string encryptedText)
    {
        if (_k.Length == 0) throw new Exception("Password not set");
        var encryptedBytes = Convert.FromBase64String(encryptedText);
        string decryptedText = null;

        using var aes = Aes.Create();
        var keyBytes = _k;
        var iv = new byte[aes.IV.Length];

        aes.Key = keyBytes;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor(aes.Key, iv);
        using var ms = new MemoryStream(encryptedBytes);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(cs);
        decryptedText = reader.ReadToEnd();

        return decryptedText;
    }


}