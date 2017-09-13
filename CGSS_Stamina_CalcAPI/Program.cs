using Nancy.Hosting.Self;
using Nancy;
using System;

namespace CGSS_Stamina_CalcAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            HostConfiguration hostConfigs = new HostConfiguration()
            {
                UrlReservations = new UrlReservations() { CreateAutomatically = true }
            };
            using (var host = new NancyHost(new Uri("http://localhost:50120"), new DefaultNancyBootstrapper(), hostConfigs))
            {
                host.Start();
                Console.WriteLine("Running on http://*:50120");
                Console.ReadLine();
            }
        }
    }
    public class HelloModule : NancyModule
    {
        public HelloModule()
        {
            Get("/", args => "RUNNING!!!");
        }
    }
}
