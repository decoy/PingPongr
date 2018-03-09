using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Examples.Complex
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) => new WebHostBuilder()
                .UseKestrel()
                // We're using logging all over to demonstrate
                .ConfigureLogging(builder => builder.AddConsole())
                .UseStartup<Startup>()
                .Build();
    }
}
