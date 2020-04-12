using System;

namespace Ledger.WebApi.Models
{
    public class AccountModel
    {
        public string Name { get; set; }
        public string FullyQualifiedName { get; set; }
        public string ParentFullyQualifiedName { get; set; }
    }
}