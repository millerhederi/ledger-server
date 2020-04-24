using System;
using Ledger.WebApi.Concept;
using Ledger.WebApi.Models;

namespace Ledger.WebApi.Requests
{
    public class GetTransactionRequest : IRequest<GetTransactionResponse>
    {
        public GetTransactionRequest(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }

    public class GetTransactionResponse
    {
        public TransactionModel Transaction { get; set; }
    }
}