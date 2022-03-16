using KLog.Api.Core;
using KLog.Api.Services;
using KLog.DataModel.Context;
using KLog.DataModel.DTOs;
using KLog.DataModel.Entities;
using Microsoft.AspNetCore.Http;
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

        /// <summary>
        /// Provides a new private / public keypair in XML format
        /// </summary>
        /// <returns>A newly generated private / public RSA keypair in XML format</returns>
        /// <remarks>
        /// Sample Request: 
        /// 
        ///     POST /api/authentication/keypair
        ///     
        /// </remarks>
        /// <response code="200">Returns the generated keypair</response>
        [HttpPost("keypair")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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

        /// <summary>
        /// Returns a public key that can be used by a web client
        /// </summary>
        /// <returns>A public RSA key</returns>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     POST /api/key
        ///     
        /// </remarks>
        /// <response code="200">Returns a generated public key</response>
        [HttpPost("key")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetPublicKey()
        {
            return ApiResponse.Success(AuthService.GeneratePublicKey());
        }

        /// <summary>
        /// Creates a new user and returns a jwt
        /// </summary>
        /// <param name="registration">An object containing an encrypted username and password</param>
        /// <returns>A jwt that can be used for authentication</returns>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     POST /api/authentication/register
        ///     {
        ///         username: "myEncryptedUsername",
        ///         password: "myEncryptedPassword"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns a jwt for the newly created user</response>
        /// <response code="400">If the username or password is missing</response>
        /// <response code="409">If a duplicate user exists</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
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

            await DbContext.AddAsync(newUser);
            await DbContext.SaveChangesAsync();
            return ApiResponse.Success(AuthService.GenerateJwt(newUser));
        }

        /// <summary>
        /// Authenticates a user and returns a jwt
        /// </summary>
        /// <param name="login">An object containing an encrypted username and password</param>
        /// <returns>A jwt that can be used for authentication</returns>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     POST /api/authentication/login
        ///     {
        ///         "username": "myEncryptedUsername",
        ///         "password": "myEncryptedPassword"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the generated jwt</response>
        /// <response code="400">If a username or password is not provided</response>
        /// <response code="401">If the provided username and password cannot be authenticated</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
