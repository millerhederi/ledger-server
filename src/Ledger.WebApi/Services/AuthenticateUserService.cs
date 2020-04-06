using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Ledger.WebApi.Concept;
using Ledger.WebApi.DataAccess;
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
        public const string UserIdClaimName = "UserId";

        private readonly IConfiguration _configuration;
        private readonly IRepository _repository;

        public AuthenticateUserService(
            IConfiguration configuration,
            IRepository repository)
        {
            _configuration = configuration;
            _repository = repository;
        }

        public async Task<string> ExecuteAsync(string userName, string password, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("UserName", userName);

            var user = await _repository
                .QuerySingleOrDefaultAsync<User>(new CommandDefinition(
                    "select * from [dbo].[User] where [UserName] = @UserName",
                    parameters,
                    cancellationToken: cancellationToken));

            // Todo: hard coding the password for now until we research hashing options
            if (user == null || password != "password")
            {
                return null;
            }

            return BuildToken(user);
        }
        
        private string BuildToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(UserIdClaimName, user.Id.ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}