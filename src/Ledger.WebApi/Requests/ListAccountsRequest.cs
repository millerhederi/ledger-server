using System.Collections.Generic;
using Ledger.WebApi.Concept;
using Ledger.WebApi.Models;

namespace Ledger.WebApi.Requests
{
    public class ListAccountsRequest : IRequest<ListAccountsResponse>
    {
    }

    public class ListAccountsResponse
    {
        public ICollection<AccountModel> Items { get; set; } = new List<AccountModel>();
    }
}