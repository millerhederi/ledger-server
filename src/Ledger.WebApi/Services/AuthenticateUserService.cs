using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Ledger.WebApi.Services
{
    public interface IAuthenticateUserService
    {
        Task<string> ExecuteAsync(string userName, string password, CancellationToken cancellationToken);
    }

    public class AuthenticateUserService : IAuthenticateUserService
    {
        private readonly IConfiguration _configuration;
        
        public AuthenticateUserService(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<string> ExecuteAsync(string userName, string password, CancellationToken cancellationToken)
        {
            // Todo: hard coding username and password until we've added a DB backend
            if (userName != "user" || password != "password")
            {
                return null;
            }

            return Task.FromResult(BuildToken());
        }
        
        private string BuildToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: null,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}