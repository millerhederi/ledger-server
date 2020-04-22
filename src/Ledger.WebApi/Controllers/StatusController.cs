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
    public class StatusController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StatusController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ping")]
        public async Task<string> GetAsync(CancellationToken cancellationToken)
        {
            return await _mediator.Send(new PingRequest(), cancellationToken);
        }
    }
}
