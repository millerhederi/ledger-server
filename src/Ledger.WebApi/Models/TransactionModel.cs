using System;
using System.Collections.Generic;

namespace Ledger.WebApi.Models
{
    public class TransactionModel
    {
        public Guid Id { get; set; }
        public DateTime PostedDate { get; set; }
        public string Description { get; set; }
        public ICollection<Posting> Postings { get; } = new List<Posting>();

        public class Posting
        {
            public Account Account { get; set; }
            public decimal Amount { get; set; }
        }

        public class Account
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }
    }
}