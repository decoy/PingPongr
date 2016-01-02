namespace PingPongr.Mediator
{
    using System.Threading.Tasks;

    public interface IRequestAsyncHandler<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest message);
    }
}
