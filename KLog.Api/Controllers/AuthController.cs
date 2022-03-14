using KLog.Api.Core;
using KLog.Api.Services;
using KLog.DataModel.Context;
using KLog.DataModel.DTOs;
using KLog.DataModel.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace KLog.Api.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthController : KLogController
    {
        private readonly IAuthenticationService AuthService;

        public AuthController(KLogContext context, IAuthenticationService authService) 
            : base(context)
        {
            AuthService = authService;
        }

        [HttpPost("keypair")]
        public IActionResult GenerateKeyPair()
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                string publicKey = rsa.ToXmlString(false);
                string privateKey = rsa.ToXmlString(true);

                return ApiResponse.Success(new
                {
                    publicKey,
                    privateKey
                });
            }
        }

        [HttpPost("key")]
        public IActionResult GetPublicKey()
        {
            return ApiResponse.Success(AuthService.GeneratePublicKey());
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Login registration)
        {
            if (
                string.IsNullOrEmpty(registration.Username)
                || string.IsNullOrEmpty(registration.Password)
            )
                return ApiResponse.BadRequest("Username and Password required");

            string plainTextUser = AuthService.Decrypt(registration.Username);
            string plainTextPass = AuthService.Decrypt(registration.Password);

            User existingUser = await DbContext.Users
                .AsNoTracking()
                .Where(u => 
                    u.Username == plainTextUser
                    || u.Email == plainTextUser
                )
                .FirstOrDefaultAsync();

            if (existingUser != null)
                return ApiResponse.Conflict("User already exists");

            User newUser = new User
            {
                Username = plainTextUser,
                Password = AuthService.GenerateHash(plainTextPass),
                ResetRequired = false,
                LastLogin = DateTimeOffset.Now,
            };

            try
            {
                await DbContext.AddAsync(newUser);
                await DbContext.SaveChangesAsync();
                return ApiResponse.Success(AuthService.GenerateJwt(newUser));
            }
            catch(Exception ex)
            {
                var pause = true;
                return ApiResponse.Success(true);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            if (
                string.IsNullOrEmpty(login.Username)
                || string.IsNullOrEmpty(login.Password)
            )
                return ApiResponse.BadRequest("Username and Password required");

            string plainTextUser = AuthService.Decrypt(login.Username);
            string plainTextPass = AuthService.Decrypt(login.Password);
            
            User dbUser = await DbContext.Users
                .Where(u =>
                    u.Username == plainTextUser
                    || u.Email == plainTextUser
                )
                .FirstOrDefaultAsync();

            if (dbUser == null || !AuthService.ValidateHash(dbUser.Password, plainTextPass))
                return ApiResponse.Unauthorized();

            dbUser.LastLogin = DateTimeOffset.Now;
            await DbContext.SaveChangesAsync();

            return ApiResponse.Success(AuthService.GenerateJwt(dbUser));
        }
    }
}
