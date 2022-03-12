using KLog.DataModel.Context;
using KLog.DataModel.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KLog.Api.Controllers
{
    [Route("api/logs")]
    public class LogController : KLogController
    {
        public LogController(KLogContext context) : base(context) { }

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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult PostLog([FromBody] Log newLog)
        {
            return new OkResult();
        }

        [HttpGet]
        public IActionResult GetLog()
        {
            return new OkObjectResult(new Log
            {
                LogId = 1,
                Level = LogLevel.Critical,
                Message = "Testing"
            });
        }
    }
}
