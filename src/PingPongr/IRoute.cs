namespace PingPongr
{
    using Mediator;
    using System.Threading.Tasks;

    public interface IRoute
    {
        string Path { get; set; }

        Task Send(IMediaTypeHandler mediaHandler, InstanceFactory factory, IRequestContext context);
    }
}
