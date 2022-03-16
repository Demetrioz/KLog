using KLog.Api.Core;
using KLog.Api.Services;
using KLog.DataModel.Context;
using KLog.DataModel.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KLog.Api.Controllers
{
    [Route("api/applications")]
    public class ApplicationController : KLogController
    {
        private readonly IAuthenticationService AuthService;

        public ApplicationController(KLogContext context, IAuthenticationService authService) 
            : base(context) 
        { 
            AuthService = authService;
        }

        /// <summary>
        /// Registers a new application and associated Api Key
        /// </summary>
        /// <param name="appName"></param>
        /// <returns>The newly created application and key</returns>
        /// <remarks>
        /// Sample Request:    
        /// 
        ///     POST /api/applications?appName=MyNewApp
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created app and api key</response>
        /// <response code="401">If the auth token is missing or incorrect</response>
        /// <response code="409">If the application name already exists</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateApplication(string appName)
        {
            (int userId, bool isApiKey) = RetrieveUserId();

            if (isApiKey)
                return ApiResponse.Unauthorized();

            Application existingApplication = await DbContext.Applications
                .AsNoTracking()
                .Where(a => a.Name == appName)
                .FirstOrDefaultAsync();

            if (existingApplication != null)
                return ApiResponse.Conflict("Application name is in use");

            (string prefix, string key) = AuthService.GenerateApiKey();

            string plainKey = $"{prefix}.{key}";
            string hashedKey = AuthService.GenerateHash(plainKey);

            Application newApp = new Application
            {
                UserId = userId,
                Name = appName,
                Id = prefix,
                Key = hashedKey
            };

            DbContext.Add(newApp);
            await DbContext.SaveChangesAsync();

            newApp.Key = plainKey;
            return ApiResponse.Created(newApp);
        }

        /// <summary>
        /// Returns a list of applications associated to the user making the request
        /// </summary>
        /// <returns>A list of applications / API keys</returns>
        /// <remarks>
        /// Sample Request:
        ///     
        ///     GET /api/applications
        ///     
        /// </remarks>
        /// <response code="200">Returns a list of applications</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetApplications()
        {
            (int userId, bool isApiKey) = RetrieveUserId();

            List<Application> userApps = await DbContext.Applications
                .AsNoTracking()
                .Where(a => 
                    isApiKey 
                        ? a.ApplicationId == userId 
                        : a.UserId == userId
                )
                .ToListAsync();

            return ApiResponse.Success(userApps);
        }

        /// <summary>
        /// Deletes the requested application / API key
        /// </summary>
        /// <param name="id"></param>
        /// <returns>true if the application was deleted</returns>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     DELETE /api/applications/3
        ///     
        /// </remarks>
        /// <response code="200">Returns true if the application was deleted</response>
        /// <response code="401">If the user is unauthenticated or tries to use an API key</response>
        /// <response code="409">If the application does not belong to the user making the request</response>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteApplication(int id)
        {
            (int userId, bool isApiKey) = RetrieveUserId();

            if (isApiKey)
                return ApiResponse.Unauthorized();

            Application dbApplication = await DbContext.Applications
                .Where(a =>
                    a.ApplicationId == id
                    && a.UserId == userId
                )
                .FirstOrDefaultAsync();

            if (dbApplication == null)
                return ApiResponse.Conflict("Invalid application");

            DbContext.Remove(dbApplication);
            await DbContext.SaveChangesAsync();

            return ApiResponse.Success(true);
        }
    }
}
