using System;

namespace Ledger.WebApi.Models
{
    public class MonthlyPostingTotalModel
    {
        public decimal Amount { get; set; }
        public int Count { get; set; }
        public DateTime Date { get; set; }
    }
}