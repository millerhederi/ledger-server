namespace Ledger.WebApi.Models
{
    public class PostingModel
    {
        public AccountModel Account { get; set; }
        public decimal Amount { get; set; }
    }
}