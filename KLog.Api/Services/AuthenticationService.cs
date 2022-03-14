using KLog.Api.Config;
using KLog.Api.Core;
using KLog.DataModel.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace KLog.Api.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly SecuritySettings Settings;

        public AuthenticationService(IOptions<SecuritySettings> options)
        {
            Settings = options.Value;
        }

        public (string, string) GenerateApiKey()
        {
            string prefix = Guid.NewGuid().ToString().Split("-")[0];
            string key = Guid.NewGuid().ToString().Replace("-", "");

            string entireKey = $"{prefix}.{key}";

            byte[] keyBytes = Encoding.UTF8.GetBytes(entireKey);
            string base64Key = Convert.ToBase64String(keyBytes);

            return (prefix, base64Key);
        }

        public string GeneratePublicKey()
        {
            using (RSACryptoServiceProvider publicProvider = new RSACryptoServiceProvider())
            {
                RSAUtility.FromXmlString(publicProvider, Settings.PublicKey);
                StringWriter publicPemKey = new StringWriter();
                RSAUtility.ExportPublicKey(publicProvider, publicPemKey);

                return publicPemKey.ToString();
            }
        }

        public string GenerateJwt(User user)
        {
            SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Settings.SecretKey));
            SigningCredentials signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new List<Claim>
            {
                new Claim("sub", user.UserId.ToString()),
                new Claim("name", user.Username),
                new Claim("reset_required", user.ResetRequired.ToString())
            };

            if (!string.IsNullOrEmpty(user.Email))
                claims.Add(new Claim("email", user.Email));

            if (!string.IsNullOrEmpty(user.Phone))
                claims.Add(new Claim("phone_number", user.Phone));

            JwtSecurityToken tokenOptions = new JwtSecurityToken(
                issuer: Settings.Issuer,
                audience: Settings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: signingCredentials
            );

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(tokenOptions);
        }

        public string GenerateHash(string key)
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

        public bool ValidateHash(string hashedKey, string plainKey)
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

        public string Decrypt(string key)
        {
            using (RSACryptoServiceProvider privateProvider = new RSACryptoServiceProvider())
            {
                byte[] encryptedKey = Convert.FromBase64String(key);

                RSAUtility.FromXmlString(privateProvider, Settings.PrivateKey);

                byte[] decryptedKey = privateProvider.Decrypt(encryptedKey, false);
                return Encoding.ASCII.GetString(decryptedKey);
            }
        }
    }
}
