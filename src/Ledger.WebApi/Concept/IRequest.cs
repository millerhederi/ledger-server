namespace Ledger.WebApi.Concept
{
    public interface IRequest<out TResponse> : MediatR.IRequest<TResponse>
    {
    }
}