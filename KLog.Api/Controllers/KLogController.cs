using KLog.DataModel.Context;
using Microsoft.AspNetCore.Mvc;

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
    }
}
