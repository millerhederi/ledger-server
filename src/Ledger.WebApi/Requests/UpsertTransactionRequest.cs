using System;
using Ledger.WebApi.Models;
using MediatR;

namespace Ledger.WebApi.Requests
{
    public class UpsertTransactionRequest : IRequest<UpsertTransactionResponse>
    {
        public TransactionModel Transaction { get; set; }
    }

    public class UpsertTransactionResponse
    {
        public Guid Id { get; set; }
    }
}