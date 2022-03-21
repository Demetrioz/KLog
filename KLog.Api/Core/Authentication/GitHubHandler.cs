using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace KLog.Api.Core.Authentication
{
    // https://docs.github.com/en/developers/webhooks-and-events/webhooks/webhook-events-and-payloads
    public class GitHubHandler : AuthenticationHandler<GitHubOptions>
    {
        private readonly IHttpContextAccessor HttpContextAccessor;
        //private readonly Services.IAuthenticationService SecurityService;
        //private readonly KLogContext DbContext;

        public GitHubHandler(
            IOptionsMonitor<GitHubOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IHttpContextAccessor httpContextAccessor
            //Services.IAuthenticationService securityService,
            //KLogContext context
        ) : base(options, logger, encoder, clock)
        {
            HttpContextAccessor = httpContextAccessor;
            //SecurityService = securityService;
            //DbContext = context;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            /*
             * X-GitHub-Event - Name of the event
             * X-GitHub-Deliver - Guid
             * X-Hub-Signature - HMAC hex digest of body, generated sha-1 hash and secret as key
             * X-Hub-Signature-256 - HMAC hex digenst of body, generated sha-256 and secret as key
             */
            throw new System.NotImplementedException();
        }
    }
}
