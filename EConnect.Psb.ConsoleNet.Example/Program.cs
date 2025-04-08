using System;
using System.Threading.Tasks;
using EConnect.Psb.Hosting;
using EConnect.Psb.Models;

namespace EConnect.Psb.ConsoleNet.Example
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter username:");
            var username = Console.ReadLine();
            Console.WriteLine("Enter password:");
            var password = Console.ReadLine();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Console.WriteLine("No username or password provided.");
                return;
            }

            try
            {
                // Example how to use psb services without HostBuilder
                using (var psb = PsbServiceHost.Create(_ =>
                       {
                           _.Username = username;
                           _.Password = password;
                           _.PsbUrl = "https://accp-psb.econnect.eu";
                           _.IdentityUrl = "https://accp-identity.econnect.eu";
                           _.ClientId = "5fac195439284072";
                           _.ClientSecret = "7608f01a7b1d4d6d8b5c909527f30569";
                           _.SubscriptionKey = "Sandbox.Accp.W2NmWFRINXokdA";
                       }))
                {
                    var res = await psb.SalesInvoice.QueryRecipientParty("0106:12345678", new[] { "0106:12345678" }).ConfigureAwait(false);
                    Console.WriteLine(res);
                }
            }
            catch (EConnectException ex)
            {
                Console.WriteLine("ERROR: [" + ex.Code + "] " + ex.Message);
            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}