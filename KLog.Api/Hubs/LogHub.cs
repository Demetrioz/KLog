using KLog.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace KLog.Api.Hubs
{
    [Authorize]
    public class LogHub : Hub
    {
        public LogHub()
        {
        }
    }
}
