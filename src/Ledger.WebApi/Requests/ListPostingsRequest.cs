using System;
using System.Collections.Generic;
using MediatR;

namespace Ledger.WebApi.Requests
{
    public class ListPostingsRequest : IRequest<ListPostingsResponse>
    {
        public ListPostingsRequest(Guid accountId)
        {
            AccountId = accountId;
        }

        public Guid AccountId { get; }
    }

    public class ListPostingsResponse
    {
        public ICollection<Posting> Items { get; set; } = new List<Posting>();

        public class Posting
        {
            public Guid Id { get; set; }
            public decimal Amount { get; set; }
            public DateTime PostedDate { get; set; }
            public string Description { get; set; }
            public Guid TransactionId { get; set; }
        }
    }
}