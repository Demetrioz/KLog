using KLog.DataModel.Context;
using KLog.DataModel.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace KLog.Api.Core.Authentication
{
    public class ApiKeyHandler : AuthenticationHandler<ApiKeyOptions>
    {
        private readonly IHttpContextAccessor HttpContextAccessor;
        private readonly Services.IAuthenticationService SecurityService;
        private readonly KLogContext DbContext;

        public ApiKeyHandler(
            IOptionsMonitor<ApiKeyOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IHttpContextAccessor httpContextAccessor,
            Services.IAuthenticationService securityService,
            KLogContext context
        ) : base(options, logger, encoder, clock)
        {
            HttpContextAccessor = httpContextAccessor;
            SecurityService = securityService;
            DbContext = context;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string apiKey = HttpContextAccessor.HttpContext.Request.Headers
                .Where(h => h.Key == "x-api-key")
                .Select(kvp => kvp.Value)
                .FirstOrDefault()
                .ToString();

            if (string.IsNullOrEmpty(apiKey))
                return AuthenticateResult.Fail("Unauthorized");

            string prefix = apiKey.Split(".")[0];

            Application app = await DbContext.Applications
                .AsNoTracking()
                .Where(a => a.Id == prefix)
                .FirstOrDefaultAsync();

            if (app != null && SecurityService.ValidateHash(app.Key, apiKey))
            {
                Claim[] claims = new[] { new Claim(ClaimTypes.Name, app.Name) };
                ClaimsIdentity identity = new ClaimsIdentity(claims, Scheme.Name);
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }

            return AuthenticateResult.Fail("Unauthorized");
        }
    }
}
