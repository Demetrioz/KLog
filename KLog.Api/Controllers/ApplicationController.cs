using KLog.Api.Core;
using KLog.Api.Services;
using KLog.DataModel.Context;
using KLog.DataModel.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace KLog.Api.Controllers
{
    [Route("api/applications")]
    public class ApplicationController : KLogController
    {
        private readonly ISecurityService SecurityService;

        public ApplicationController(KLogContext context, ISecurityService securityService) 
            : base(context) 
        { 
            SecurityService = securityService;
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateApplication(string appName)
        {
            Application existingApplication = await DbContext.Applications
                .AsNoTracking()
                .Where(a => a.Name == appName)
                .FirstOrDefaultAsync();

            if (existingApplication != null)
                return ApiResponse.Conflict("Application name is in use");

            (string prefix, string key) = SecurityService.GenerateKey();

            string plainKey = $"{prefix}.{key}";
            string hashedKey = SecurityService.GenerateKeyHash(plainKey);

            Application newApp = new Application
            {
                Name = appName,
                Id = prefix,
                Key = hashedKey
            };

            DbContext.Add(newApp);
            await DbContext.SaveChangesAsync();

            newApp.Key = plainKey;
            return ApiResponse.Created(newApp);
        }
    }
}
