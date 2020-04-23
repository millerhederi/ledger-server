using System;
using System.Collections.Generic;
using MediatR;

namespace Ledger.WebApi.Requests
{
    public class GetPostingAggregatesRequest : IRequest<GetPostingAggregatesResponse>
    {
        public GetPostingAggregatesRequest(Guid accountId)
        {
            AccountId = accountId;
        }

        public Guid AccountId { get; }
    }

    public class GetPostingAggregatesResponse
    {
        public ICollection<Aggregate> Aggregates { get; set; } = new List<Aggregate>();

        public class Aggregate
        {
            public decimal Amount { get; set; }
            public int Count { get; set; }
            public DateTime Date { get; set; }
        }
    }
}