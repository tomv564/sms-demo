using System;
using Nancy.Hosting.Self;

namespace SMSServer
{
    class Program
    {
        const string StagingPort = "8080";

        static readonly string HOST = Environment.GetEnvironmentVariable("HOST");
        static readonly string PORT = Environment.GetEnvironmentVariable("PORT");
        //static readonly string ELKSUSER = Environment.GetEnvironmentVariable("ELKSUSER");
        //static readonly string ELKSPASSWORD = Environment.GetEnvironmentVariable("ELKSPASSWORD");

        static NancyHost Host;

        enum Env { Staging, Deployment }

        static Env CurrentEnv
        {
            get
            {
                return PORT == null ? Env.Staging : Env.Deployment;
            }
        }

        static Uri CurrentAddress
        {
            get
            {
                switch (CurrentEnv)
                {
                    case Env.Staging:
                        return new Uri("http://localhost:" + StagingPort);
                    case Env.Deployment:
                        var host = string.IsNullOrEmpty(HOST) ? "localhost" : HOST;
                        return new Uri("http://" + host + ":" + PORT)
                    default:
                        throw new Exception("Unexpected environment");
                }
            }
        }

        //static async Task<string> CreateSMSNumber(string path)
        //{
        //    var client = new FourtySixElksClient.ApiClient(ELKSUSER, ELKSPASSWORD);
        //    return await client.CreateNumberAsync("se", new Uri(CurrentAddress, path), null, null);
        //}

        static void Main(string[] args)
        {

            Console.WriteLine("Configuring for address {0}", CurrentAddress);
            var config = new HostConfiguration() {UrlReservations = new UrlReservations {CreateAutomatically = true}};
            Host = new NancyHost(config, CurrentAddress);
            Host.Start();
            Console.WriteLine("Nancy is started and listening on {0}...", CurrentAddress);

            // Example to automatically register a phone number.
            // In this case it's much handier to configure the number manually in the dashboard.
            //Task.Run(async () =>
            //{

            //    var number = await CreateSMSNumber("sms/echo");
            //    Console.WriteLine("sms/echo SMS handler registered for {0}", number);

            //}).Wait();

            for (var line = Console.ReadLine (); line != "quit";);

            Host.Stop();
        }
    }
}
