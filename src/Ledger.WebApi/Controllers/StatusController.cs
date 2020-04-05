using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("ping")]
        public string Get()
        {
            return "pong";
        }
    }
}
