using KLog.Api.Core;
using KLog.Api.Hubs;
using KLog.DataModel.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace KLog.Api.Controllers
{
    [Route("api/github")]
    public class GitHubController : KLogController
    {
        public GitHubController(IHubContext<LogHub> hub, KLogContext context) : base(hub, context)
        {
        }

        [HttpPost]
        public async Task<IActionResult> ParseGithubEvent()
        {
            return ApiResponse.Success(true);
        }
    }
}
