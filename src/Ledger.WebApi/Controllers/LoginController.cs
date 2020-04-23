using System.Threading;
using System.Threading.Tasks;
using Ledger.WebApi.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController
    {
        private readonly IMediator _mediator;

        public LoginController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<BuildUserJwtTokenResponse>> CreateTokenAsync([FromBody] BuildUserJwtTokenRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            
            if (response.Token == null)
            {
                return new UnauthorizedResult();
            }

            return new OkObjectResult(response);
        }
    }
}