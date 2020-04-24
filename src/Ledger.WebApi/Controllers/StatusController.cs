using System.Threading;
using System.Threading.Tasks;
using Ledger.WebApi.Concept;
using Ledger.WebApi.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly IRequestProcessingPipeline _pipeline;

        public StatusController(IRequestProcessingPipeline pipeline)
        {
            _pipeline = pipeline;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ping")]
        public async Task<ResponseEnvelope<string>> GetAsync(CancellationToken cancellationToken)
        {
            return await _pipeline.ExecuteAsync(new PingRequest(), cancellationToken);
        }
    }
}
