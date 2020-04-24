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
    public class LoginController
    {
        private readonly IRequestProcessingPipeline _pipeline;

        public LoginController(IRequestProcessingPipeline pipeline)
        {
            _pipeline = pipeline;
        }
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<ResponseEnvelope<BuildUserJwtTokenResponse>>> CreateTokenAsync([FromBody] BuildUserJwtTokenRequest request, CancellationToken cancellationToken)
        {
            var response = await _pipeline.ExecuteAsync(request, cancellationToken);
            
            if (response.Data?.Token == null)
            {
                return new UnauthorizedResult();
            }

            return new OkObjectResult(response);
        }
    }
}