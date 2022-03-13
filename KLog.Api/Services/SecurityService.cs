using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using System.Text;

namespace KLog.Api.Services
{
    public class SecurityService : ISecurityService
    {
        public (string, string) GenerateKey()
        {
            string prefix = Guid.NewGuid().ToString().Split("-")[0];
            string key = Guid.NewGuid().ToString().Replace("-", "");

            string entireKey = $"{prefix}.{key}";

            byte[] keyBytes = Encoding.UTF8.GetBytes(entireKey);
            string base64Key = Convert.ToBase64String(keyBytes);

            return (prefix, base64Key);
        }

        public string GenerateKeyHash(string key)
        {
            // Generate salt
            byte[] salt;

            // replaced RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            salt = RandomNumberGenerator.GetBytes(16);

            byte[] pbkdf2 = KeyDerivation.Pbkdf2(key, salt, KeyDerivationPrf.HMACSHA256, 10000, 20);

            // Combine the two 
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(pbkdf2, 0, hashBytes, 16, 20);

            return Convert.ToBase64String(hashBytes);
        }

        public bool ValidateKey(string hashedKey, string plainKey)
        {
            byte[] hashedBytes = Convert.FromBase64String(hashedKey);

            byte[] salt = new byte[16];
            Array.Copy(hashedBytes, 0, salt, 0, 16);

            var pbkdf2 = KeyDerivation.Pbkdf2(plainKey, salt, KeyDerivationPrf.HMACSHA256, 10000, 20);

            for (int i = 0; i < 20; i++)
                if (hashedBytes[i + 16] != pbkdf2[i])
                    return false;

            return true;
        }
    }
}
