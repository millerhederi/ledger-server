namespace Ledger.WebApi.Concept
{
    public class ResponseEnvelope<TResponse>
    {
        public TResponse Data { get; set; }
        public string Error { get; set; }
    }
}