using System;
using Ledger.WebApi.Models;
using MediatR;

namespace Ledger.WebApi.Requests
{
    public class UpsertAccountRequest : IRequest<UpsertAccountResponse>
    {
        public AccountModel Account { get; set; }
    }

    public class UpsertAccountResponse
    {
        public Guid Id { get; set; }
    }
}