using System.Security.Cryptography;

namespace IncidentsAppApi
{
    public static class Encryptor
    {
        private static byte[] key;
        private static byte[] iv;

        public static void Init()
        {
            // Generate and save keys on first run
            if (!File.Exists("encryption.key"))
            {
                byte[] key = new byte[32]; // 256-bit
                byte[] iv = new byte[16];  // 128-bit

                Random random = new Random();
                random.NextBytes(key);
                random.NextBytes(iv);

                // Store protected
                byte[] protectedKey = ProtectedData.Protect(key, null, DataProtectionScope.CurrentUser);
                byte[] protectedIv = ProtectedData.Protect(iv, null, DataProtectionScope.CurrentUser);

                File.WriteAllBytes("encryption.key", protectedKey);
                File.WriteAllBytes("encryption.iv", protectedIv);
            }

            key = ProtectedData.Unprotect(File.ReadAllBytes("encryption.key"), null, DataProtectionScope.CurrentUser);
            iv = ProtectedData.Unprotect(File.ReadAllBytes("encryption.iv"), null, DataProtectionScope.CurrentUser);
        }

        public static string Decrypt(byte[] cipherText)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new MemoryStream(cipherText);
            using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();
        }

        public static byte[] Encrypt(string plainText)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msEncrypt = new();
            using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                using StreamWriter swEncrypt = new(csEncrypt);
                swEncrypt.Write(plainText);
            }
            return msEncrypt.ToArray();
        }

        public static string EncryptToString(string plainText)
        {
            byte[] encrypted = Encrypt(plainText);
            return Convert.ToBase64String(encrypted);
        }

        public static string DecryptFromString(string encryptedString)
        {
            byte[] decryptedString = Convert.FromBase64String(encryptedString);
            return Decrypt(decryptedString);
        }
    }
}
