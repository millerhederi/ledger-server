using System;

namespace Ledger.WebApi.DataAccess
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime PostedDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public DateTime UpdatedTimestamp { get; set; }
    }
}