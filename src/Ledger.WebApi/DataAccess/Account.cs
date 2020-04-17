using System;
using Ledger.WebApi.Concept;

namespace Ledger.WebApi.DataAccess
{
    public class Account : IEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public DateTime UpdatedTimestamp { get; set; }
    }
}