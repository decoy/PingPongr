namespace PingPongr.Sandbox
{
    using Owin;
    using OwinSupport;
    using System;
    using System.Linq;

    public class Program
    {
        public class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                var routes = RouteBuilder
                    .WithAssemblies(typeof(Program).Assembly)  //limit our search to this assembly
                    .Filter(t => t.FullName.StartsWith("PingPongr.Sandbox.Api")) //limit to api namespace
                    .Path(t => "/" + t.Name) //override default path naming
                    .GetRoutes();

                foreach (var r in routes) Console.WriteLine(r.Path);

                //build the router - this could be done using a proper container
                var router = new Router(
                    routes,
                    new[] { new DefaultJsonMediaHandler() },
                    SingleInstanceFactory);

                

                app.UsePingPongr(router, "/api");

                app.UseWelcomePage("/");
            }
        }

        public static void Main(string[] args)
        {
            using (Microsoft.Owin.Hosting.WebApp.Start<Startup>("http://localhost:12345"))
            {
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Pretends to be an IOC container providing new types
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        private static object SingleInstanceFactory(Type serviceType)
        {
            var type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .First(serviceType.IsAssignableFrom);
            return Activator.CreateInstance(type);
        }
    }
}
