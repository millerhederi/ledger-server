using System.Threading;
using System.Threading.Tasks;
using Ledger.WebApi.Models;
using Ledger.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController
    {
        private readonly IAuthenticateUserService _authenticateUserService;

        public LoginController(IAuthenticateUserService authenticateUserService)
        {
            _authenticateUserService = authenticateUserService;
        }
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateTokenAsync([FromBody] LoginModel login, CancellationToken cancellationToken)
        {
            var token = await _authenticateUserService.ExecuteAsync(login.UserName, login.Password, cancellationToken);
            
            if (token == null)
            {
                return new UnauthorizedResult();
            }

            return new OkObjectResult(new {Token = token});
        }
    }
}