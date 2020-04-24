using System.Collections.Generic;
using Ledger.WebApi.Concept;
using Ledger.WebApi.Models;

namespace Ledger.WebApi.Requests
{
    public class ListTransactionsRequest : IRequest<ListTransactionsResponse>
    {
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 50;
    }

    public class ListTransactionsResponse
    {
        public ICollection<TransactionModel> Items { get; set; } = new List<TransactionModel>();
    }
}