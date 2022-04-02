using KLog.Api.Core;
using KLog.Api.Core.Queries;
using KLog.Api.Extensions;
using KLog.Api.Hubs;
using KLog.DataModel.Context;
using KLog.DataModel.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
            (int userId, bool isApiKey) = RetrieveUserId();

            if (!isApiKey)
                return ApiResponse.Unauthorized();

            newLog.Source = User.Identity.Name;
            newLog.ApplicationId = userId;

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
                List<Application> applications = await DbContext.Applications
                    .AsNoTracking()
                    .Where(a => a.UserId == userId)
                    .ToListAsync();

                List<string> appNames = applications.Select(a => a.Name).ToList();
                List<int> appIds = applications.Select(a => a.ApplicationId).ToList();

                // If there is no query source, return logs from all api keys the user has created
                if (string.IsNullOrEmpty(query.Source))
                    logs = logs.Where(l => 
                        appNames.Contains(l.Source) 
                        && appIds.Contains(l.ApplicationId)
                    );

                // If there is a query source, get all sources requested, as long as they were created
                // by the user.
                else
                {
                    List<string> sources = query.Source.Split(',').ToList();
                    List<string> sourceNames = new List<string>();
                    List<int> sourceIds = new List<int>();

                    foreach(string source in sources)
                    {
                        if (int.TryParse(source, out int sourceInt))
                            sourceIds.Add(sourceInt);
                        else
                            sourceNames.Add(source);
                        
                    }

                    if (sourceNames.Count > 0)
                        logs = logs.Where(l => 
                            sourceNames.Contains(l.Source) 
                            && appIds.Contains(l.ApplicationId)
                        );

                    if(sourceIds.Count > 0)
                        logs = logs.Where(l => 
                            sourceIds.Contains(l.ApplicationId) 
                            && appIds.Contains(l.ApplicationId)
                        );
                }
            }

            if (query.LogLevel != null)
                logs = logs.Where(l => l.Level == query.LogLevel);

            if (query.StartTime != null)
                logs = logs.Where(l => l.Timestamp >= query.StartTime);

            if (query.StopTime != null)
                logs = logs.Where(l => l.Timestamp <= query.StopTime);

            if(!string.IsNullOrEmpty(query.SearchText))
            {
                List<string> availableFields = typeof(Log)
                         .GetProperties()
                         .Where(p => p.PropertyType == typeof(string))
                         .Select(p => p.Name)
                         .ToList();

                var searchFields = string.IsNullOrEmpty(query.SearchFields)
                    ? availableFields
                    : query.SearchFields
                        .Split(",")
                        .ToList();

                List<Expression<Func<Log, bool>>> filters = new List<Expression<Func<Log, bool>>>();
                try
                {
                    foreach (var field in searchFields)
                    {
                        if(availableFields.Contains(field))
                        {
                            // https://entityframeworkcore.com/knowledge-base/49003404/how-to-create-linq-expression-dynamically-in-csharp-for-contains
                            MethodInfo method = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
                            ParameterExpression parameter = Expression.Parameter(typeof(Log), "Log");
                            MemberExpression member = Expression.Property(parameter, field);
                            ConstantExpression constant = Expression.Constant(query.SearchText);
                            Expression body = Expression.Call(member, method, constant);
                            filters.Add(Expression.Lambda<Func<Log, bool>>(body, parameter));
                        }
                    }

                    if(filters.Count > 0)
                    {
                        Expression<Func<Log, bool>> combinedFilters = filters.Aggregate((x, y) => x.OrElse(y));logs = logs.Where(combinedFilters);
                        logs = logs.Where(combinedFilters);
                    }
                }
                catch(Exception ex)
                {
                    var pause = true;
                }
            }

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