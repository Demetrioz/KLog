using KLog.Api.Core.Queries;
using KLog.DataModel.Context;
using KLog.DataModel.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace KLog.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class KLogController : ControllerBase
    {
        protected KLogContext DbContext { get; set; }

        public KLogController(KLogContext context) 
        {
            DbContext = context;
        }

        protected PaginatedResult<T> ToPaginatedResult<T>(
            IQueryable<T> items,
            int pageNumber,
            int pageSize
        ) 
            where T : KLogBase
        {
            string scheme = HttpContext.Request.Scheme;
            QueryString queryString = HttpContext.Request.QueryString;
            HostString host = HttpContext.Request.Host;
            PathString path = HttpContext.Request.Path;
            var requestUrl = $"{scheme}://{host}{path}{queryString}";

            return PaginatedResult<T>
                .ToPaginatedResult(items, pageNumber, pageSize, requestUrl);
        }

        protected (int, bool) RetrieveUserId()
        {
            string stringId = User.Claims
                .Where(c => c.Type == "sub" || c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value)
                .FirstOrDefault();

            string authScheme = User.Claims
                .Where(c => c.Type == "authentication_method")
                .Select(c => c.Value)
                .FirstOrDefault();

            int.TryParse(stringId, out int userId);

            return (userId, authScheme == "ApiKey");
        }
    }
}
