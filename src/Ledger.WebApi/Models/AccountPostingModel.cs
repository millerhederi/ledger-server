using System;

namespace Ledger.WebApi.Models
{
    public class AccountPostingModel
    {
        public Guid Id { get; set; }
        public Guid TransactionId { get; set; }
        public string Description { get; set; }
        public DateTime PostedDate { get; set; }
        public decimal Amount { get; set; }
    }
}