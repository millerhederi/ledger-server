using System;
using System.Collections.Generic;

namespace Ledger.WebApi.Models
{
    public class TransactionModel
    {
        public Guid Id { get; set; }
        public DateTime PostedDate { get; set; }
        public string Description { get; set; }
        public ICollection<PostingModel> Postings { get; } = new List<PostingModel>();
    }
}