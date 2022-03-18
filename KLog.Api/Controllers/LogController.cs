using KLog.Api.Core;
using KLog.Api.Core.Queries;
using KLog.Api.Hubs;
using KLog.DataModel.Context;
using KLog.DataModel.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KLog.Api.Controllers
{
    [Route("api/logs")]
    public class LogController : KLogController
    {
        public LogController(IHubContext<LogHub> hub, KLogContext context) 
            : base(hub, context) {}

        /// <summary>
        /// Creates a new log entry
        /// </summary>
        /// <param name="newLog"></param>
        /// <returns>The newly created log</returns>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     POST /api/logs
        ///     {
        ///         "timestamp": "2022-01-01 00:00:00 -05:00"
        ///         "level": "Info",,
        ///         "source": "GitHub",
        ///         "subject": "Pull Request",
        ///         "component": "Repo XYZ"
        ///         "message": "My new log",
        ///         "data": "{ \"my_data\": \"some json\" }"
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created log</response>
        /// <response code="401">If the application's API key is missing or incorrect</response>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "ApiKey")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PostLog([FromBody] Log newLog)
        {
            newLog.Source = User.Identity.Name;

            await DbContext.AddAsync(newLog);
            await DbContext.SaveChangesAsync();

            await NotifyUser("PublishLog", newLog);

            return ApiResponse.Created(newLog);
        }

        /// <summary>
        /// Retrieves a paginated list of logs, based on the query parameters
        /// </summary>
        /// <param name="query"></param>
        /// <returns>A paginated list of logs and page information</returns>
        /// <response code="200">Returns a list of logs, ordered by timestamp</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLogs([FromQuery] LogQueryParams query)
        {
            (int userId, bool isApiKey) = RetrieveUserId();

            IQueryable<Log> logs = DbContext.Logs.AsQueryable();

            if (isApiKey)
            {
                string appName = await DbContext.Applications
                    .AsNoTracking()
                    .Where(a => a.ApplicationId == userId)
                    .Select(a => a.Name)
                    .FirstOrDefaultAsync();

                // If using an API key, only return logs created by that key
                logs = logs.Where(l => l.Source == appName);
            }
            else
            {
                List<string> appNames = await DbContext.Applications
                    .AsNoTracking()
                    .Where(a => a.UserId == userId)
                    .Select(a => a.Name)
                    .ToListAsync();

                // If there is no query source, return logs from all api keys the user has created
                if (string.IsNullOrEmpty(query.Source))
                    logs = logs.Where(l => appNames.Contains(l.Source));

                // If there is a query source, and that source was created by the user, show them
                else if (appNames.Contains(query.Source))
                    logs = logs.Where(l => l.Source == query.Source);

                // If we have a source, but it isn't one of the users, return nothing
                // (All logs have a source from line 57)
                else
                    logs = logs.Where(l => l.Source == null);
            }

            if (query.LogLevel != null)
                logs = logs.Where(l => l.Level == query.LogLevel);

            if (query.StartTime != null)
                logs = logs.Where(l => l.Timestamp >= query.StartTime);

            if (query.StopTime != null)
                logs = logs.Where(l => l.Timestamp <= query.StopTime);

            logs = query.MostRecent
                ? logs.OrderByDescending(l => l.Timestamp)
                : logs.OrderBy(l => l.Timestamp);

            PaginatedResult<Log> result = ToPaginatedResult(
                logs,
                query.Page,
                query.PageSize
            );

            return ApiResponse.Success(result);
        }
    }
}