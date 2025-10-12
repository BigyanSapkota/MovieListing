using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Shared.Helper
{
     public class EncryptDecryptHelper
    {
        private readonly IConfiguration _configuration;

        public EncryptDecryptHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string Encrypt(string plainText,Guid orgId, string masterKey)
        {
            string combinedKey = masterKey + orgId;

            byte[] keyBytes = Encoding.UTF8.GetBytes(combinedKey);
            if (keyBytes.Length != 32)
            {
                Array.Resize(ref keyBytes, 32);
            }
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }




        public static string Decrypt(string encryptedText, string orgId, string masterKey)
        {
            string combinedKey = masterKey + orgId;
            byte[] keyBytes = Encoding.UTF8.GetBytes(combinedKey);
            if (keyBytes.Length != 32)
            {
                Array.Resize(ref keyBytes, 32);
            }
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }



        }
}
