using KLog.Api.Hubs;
using KLog.DataModel.Context;
using KLog.DataModel.DTOs.GitHub;
using KLog.DataModel.DTOs.GitHub.Events;
using KLog.DataModel.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
                Timestamp = DateTimeOffset.UtcNow,
                Level = EventLevel,
                ApplicationId = appId,
                Source = EventSource,
            };

            switch(eventObject)
            {
                case Ping ping:
                    await HandlePing(baseLog, ping);
                    break;
                case CodeScanAlert alert:
                    await HandleAlert(baseLog, alert);
                    break;
                case CommitComment comment:
                    await HandleCommitComment(baseLog, comment);
                    break;
                case Create create:
                    await HandleCreate(baseLog, create);
                    break;
                case Delete delete:
                    await HandleDelete(baseLog, delete);
                    break;
                case Fork fork:
                    await HandleFork(baseLog, fork);
                    break;
                case IssueComment issueComment:
                    await HandleIssueComment(baseLog, issueComment);
                    break;
                case Issues issues:
                    await HandleIssues(baseLog, issues);
                    break;
                case LabelEvent labelEvent:
                    await HandleLabelEvent(baseLog, labelEvent);
                    break;
                case PullRequestEvent prEvent:
                    await HandlePrEvent(baseLog, prEvent);
                    break;
                case RepositoryVulnerability vulnerability:
                    await HandleVulnerabilityAlert(baseLog, vulnerability);
                    break;
                case Star star:
                    await HandleStar(baseLog, star);
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

        public async Task SaveLog(Log log)
        {
            await DbContext.AddAsync(log);
            await DbContext.SaveChangesAsync();
            await NotifyUser(log);
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
            
            await SaveLog(log);
        }

        public async Task HandleAlert(Log log, CodeScanAlert alert)
        {
            log.Subject = "CodeScanAlert";
            log.Component = alert.Repo.Name;
            log.Message = $"A security vulnerability has been found in {alert.Repo.Name}!";
            log.Data = JsonConvert.SerializeObject(alert.Alert);
            log.Level = LogLevel.Critical;

            await SaveLog(log);
        }

        public async Task HandleCommitComment(Log log, CommitComment comment)
        {
            log.Subject = "Comment";
            log.Component = comment.Repo.Name;
            log.Message = $"{comment.Sender.Login} commented on {comment.Repo.FullName} at {comment.Comment.Created}";

            await SaveLog(log);
        }

        public async Task HandleCreate(Log log, Create create)
        {
            log.Subject = "Create";
            log.Component = create.Repo.Name;
            log.Message = $"{create.Sender.Login} created {create.RefType} {create.Ref} on {create.Repo.FullName}";

            await SaveLog(log);
        }

        public async Task HandleDelete(Log log, Delete delete)
        {
            log.Subject = "Delete";
            log.Component = delete.Repo.Name;
            log.Message = $"{delete.Sender.Login} deleted {delete.RefType} {delete.Ref} on {delete.Repo.FullName}";
            log.Level = LogLevel.Warning;

            await SaveLog(log);
        }

        public async Task HandleFork(Log log, Fork fork)
        {
            log.Subject = "Fork";
            log.Component = fork.Repo.Name;
            log.Message = $"{fork.Sender.Login} forked {fork.Repo.Name} to {fork.Forkee.HTMLUrl}";

            await SaveLog(log);
        }

        public async Task HandleIssueComment(Log log, IssueComment issueComment)
        {
            log.Subject = "Comment";
            log.Component = issueComment.Issue.URL;
            log.Message = $"{issueComment.Sender.Login} commented on issue {issueComment.Issue.Number} " +
                $"from {issueComment.Issue.Repo.Name}";

            await SaveLog(log);
        }

        public async Task HandleIssues(Log log, Issues issues)
        {
            string component = issues.Action == "opened"
                ? issues.Repo.FullName
                : issues.Issue.URL;

            log.Subject = "Issue";
            log.Component = component;
            log.Message = $"{issues.Sender.Login} {issues.Action} issue {issues.Issue.Number} " +
                $"on {issues.Repo.FullName}";

            await SaveLog(log);            
        }

        public async Task HandleLabelEvent(Log log, LabelEvent labelEvent)
        {
            log.Subject = "Label";
            log.Component = $"{labelEvent.Repo.Name}/{labelEvent.Label?.Name}";
            log.Message = $"{labelEvent.Sender.Login} {labelEvent.Action} label {labelEvent.Label?.Name} " +
                $"on {labelEvent.Repo.FullName}";

            await SaveLog(log);
        }

        public async Task HandlePrEvent(Log log, PullRequestEvent prEvent)
        {
            log.Subject = "PullRequest";
            log.Component = prEvent.Repo.Name;
            log.Message = $"PR {prEvent.PR.Number} was {prEvent.Action} for {prEvent.Repo.Name}";

            await SaveLog(log);
        }

        public async Task HandleVulnerabilityAlert(Log log, RepositoryVulnerability vulnerability)
        {
            log.Subject = "Vulnerability";
            log.Component = vulnerability.Repo.Name;
            log.Level = LogLevel.Critical;
            log.Message = $"Dependabot has found vulnerability {vulnerability.Alert.Identifier} in repo" +
                $" {vulnerability.Repo.Name}! This is resolved in version {vulnerability.Alert.FixedIn} and " +
                $"more information is available at {vulnerability.Alert.Reference}!";

            await SaveLog(log);
        }

        public async Task HandleStar(Log log, Star star)
        {
            log.Subject = "Star";
            log.Component = star.Repo.Name;
            log.Message = $"{star.Sender.Login} starred {star.Repo.Name}!";

            await SaveLog(log);
        }
        public async Task HandleDefaultEvent(Log log, GitHubEvent e, string eventType)
        {
            log.Subject = e.Action;
            log.Message = $"{e.Sender.Login} from {e.Org.Login} performed {eventType} on {e.Repo.Name }";

            await SaveLog(log);
        }
    }
}
