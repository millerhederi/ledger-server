using System;
using MediatR;

namespace Ledger.WebApi.Requests
{
    public class AddUserRequest : IRequest<AddUserResponse>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class AddUserResponse
    {
        public Guid Id { get; set; }
    }
}