using Microsoft.AspNetCore.Mvc;
using System;

namespace KLog.Api.Core
{
    public class KLogApiResponse
    {
        public Guid RequestId { get; set; }
        public object Data { get; set; }
        public string Error { get; set; }
    }

    public static class ApiResponse
    {
        public static IActionResult Success(object data) =>
            new OkObjectResult(new KLogApiResponse
            {
                RequestId = Guid.NewGuid(),
                Data = data,
                Error = null
            });

        public static IActionResult Created(object data) =>
            new CreatedResult("created", new KLogApiResponse
            {
                RequestId = Guid.NewGuid(),
                Data = data,
                Error = null
            });

        public static IActionResult Conflict(string error) =>
            new ConflictObjectResult(new KLogApiResponse
            {
                RequestId = Guid.NewGuid(),
                Data = null,
                Error = error
            });

        public static IActionResult Unauthorized() =>
            new UnauthorizedObjectResult(new KLogApiResponse
            {
                RequestId = Guid.NewGuid(),
                Data = null,
                Error = "Unauthorized"
            });
    }
}
