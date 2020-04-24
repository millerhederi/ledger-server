using System;
using Ledger.WebApi.Concept;
using Ledger.WebApi.Models;

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