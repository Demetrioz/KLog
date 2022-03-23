using KLog.Api.Hubs;
using KLog.DataModel.Context;
using KLog.DataModel.DTOs.GitHub;
using KLog.DataModel.DTOs.GitHub.Events;
using KLog.DataModel.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KLog.Api.Services
{
    public class GitHubEventHandler : IGitHubEventHandler
    {
        private readonly IHubContext<LogHub> Hub;
        private readonly KLogContext DbContext;

        private const string EventSource = "GitHub";
        private const LogLevel EventLevel = LogLevel.Info;

        public GitHubEventHandler(IHubContext<LogHub> hub, KLogContext dbContext)
        {
            Hub = hub;
            DbContext = dbContext;
        }

        public async Task HandleEvent(int appId, object eventObject, string eventType = null)
        {
            Log baseLog = new Log
            {
                Timestamp = DateTimeOffset.Now,
                Level = EventLevel,
                ApplicationId = appId,
                Source = EventSource,
            };

            switch(eventObject)
            {
                case Ping p:
                    await HandlePing(baseLog, p);
                    break;
                default:
                    try
                    {
                        GitHubEvent gitHubEvent = eventObject as GitHubEvent;
                        await HandleDefaultEvent(baseLog, gitHubEvent, eventType);
                    }
                    catch(Exception ex)
                    {
                        // TODO: Add a system log. Who should have access to these? 
                        // The first user? First user can be "Admin" and control access
                        // of other users?
                    }
                    break;
            }
        }

        public async Task NotifyUser(Log log)
        {
            string userIdString = await DbContext.Applications
                .AsNoTracking()
                .Where(a => a.ApplicationId == log.ApplicationId)
                .Select(a => a.UserId.ToString())
                .FirstOrDefaultAsync();

            await Hub.Clients.User(userIdString).SendAsync("PublishLog", log);
        }

        public async Task HandlePing(Log log, Ping p)
        {
            log.Subject = "Ping";
            log.Component = p.HookId.ToString();
            log.Message = $"Application {p.HookId} pings with {p.Zen}";
            
            await DbContext.AddAsync(log);
            await DbContext.SaveChangesAsync();
            await NotifyUser(log);
        }

        public async Task HandleDefaultEvent(Log log, GitHubEvent e, string eventType)
        {
            log.Subject = e.Action;
            log.Message = $"{e.Sender.Login} from {e.Org.Login} performed {eventType} on {e.Repo.Name }";

            await DbContext.AddAsync(log);
            await DbContext.SaveChangesAsync();
            await NotifyUser(log);
        }
    }
}
