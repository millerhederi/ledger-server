using System;
using Ledger.WebApi.Models;
using MediatR;

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