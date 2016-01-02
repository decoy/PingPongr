namespace PingPongr
{
    using System.Threading.Tasks;

    public interface IRouter
    {
        Task RouteRequest(IRequestContext context);
    }
}