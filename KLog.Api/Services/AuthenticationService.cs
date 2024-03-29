﻿using KLog.Api.Config;
using KLog.Api.Core;
using KLog.DataModel.Context;
using KLog.DataModel.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KLog.Api.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly SecuritySettings Settings;
        private readonly KLogContext DbContext;

        public AuthenticationService(
            IOptions<SecuritySettings> options, 
            KLogContext dbContext
        )
        {
            Settings = options.Value;
            DbContext = dbContext;
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
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("sub", user.UserId.ToString()),
                new Claim("name", user.Username),
                new Claim("authentication_method", "JWT"),
                new Claim("reset_required", user.ResetRequired.ToString())
            };

            if (!string.IsNullOrEmpty(user.Email))
                claims.Add(new Claim("email", user.Email));

            if (!string.IsNullOrEmpty(user.Phone))
                claims.Add(new Claim("phone", user.Phone));

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

        public JwtSecurityToken DecodeJwt(string jwt)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(jwt);
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

        public async Task<int> ValidateGithubSignature(string text, string key)
        {
            // Figure out a better way to do this. Since we could have more than one
            // user, and multiple users could integrate with GitHub, every time a
            // github request shows up, we have to go through each GitHub application / key
            // to see if the webhook signature is valid
            int result = 0;
            List<Application> dbGithubKeys = await DbContext.Applications
                .AsNoTracking()
                .Where(a => a.Name == "GitHub")
                .ToListAsync();

            for(int i = 0; i < dbGithubKeys.Count; i++)
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] textBytes = encoding.GetBytes(text);
                Byte[] keyBytes = encoding.GetBytes(dbGithubKeys[i].Id);

                Byte[] hashBytes;

                using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                    hashBytes = hash.ComputeHash(textBytes);

                string generatedKey = BitConverter
                    .ToString(hashBytes)
                    .Replace("-", "")
                    .ToLower();

                if(generatedKey == key)
                {
                    result = dbGithubKeys[i].ApplicationId;
                    break;
                };
            }

            return result;
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
