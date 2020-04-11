using System;
using Ledger.WebApi.Concept;

namespace Ledger.WebApi.DataAccess
{
    public class User : IEntity
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public DateTime UpdatedTimestamp { get; set; }
    }
}