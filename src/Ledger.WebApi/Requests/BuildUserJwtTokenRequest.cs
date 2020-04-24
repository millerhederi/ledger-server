
using Ledger.WebApi.Concept;

namespace Ledger.WebApi.Requests
{
    public class BuildUserJwtTokenRequest : IRequest<BuildUserJwtTokenResponse>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class BuildUserJwtTokenResponse
    {
        public string Token { get; set; }
    }
}