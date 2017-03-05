namespace PingPongr.Sandbox
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;

    public class Program
    {
        public static void Main(string[] args)
        {
            using (var host = CreateHost())
            {
                host.Run();
            }
        }

        private static IWebHost CreateHost()
        {
            return new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .UseUrls("http://localhost:12345")
                .Build();
        }
    }
}
