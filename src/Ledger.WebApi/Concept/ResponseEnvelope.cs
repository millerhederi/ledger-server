namespace Ledger.WebApi.Concept
{
    public class ResponseEnvelope<TResponse>
    {
        public TResponse Data { get; private set; }
        public string Error { get; private set; }

        public static ResponseEnvelope<TResponse> OkResponse(TResponse data)
        {
            return new ResponseEnvelope<TResponse>
            {
                Data = data,
            };
        }

        public static ResponseEnvelope<TResponse> ErrorResponse(string error)
        {
            return new ResponseEnvelope<TResponse>
            {
                Error = error,
            };
        }
    }
}