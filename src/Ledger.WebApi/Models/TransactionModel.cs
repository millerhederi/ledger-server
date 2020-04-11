﻿using System;

namespace Ledger.WebApi.Models
{
    public class TransactionModel
    {
        public Guid Id { get; set; }
        public DateTime PostedDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }
}