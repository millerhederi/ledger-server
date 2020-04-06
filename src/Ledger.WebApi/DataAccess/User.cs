using System;

namespace Ledger.WebApi.DataAccess
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public DateTime UpdatedTimestamp { get; set; }
    }
}