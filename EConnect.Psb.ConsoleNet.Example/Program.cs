﻿using System;
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
                           _.ClientId = "2210f77eed3a4ab2";
                           _.ClientSecret = "ddded83702534a6c9cadde3d1bf3e94a";
                           _.SubscriptionKey = "Sandbox.Accp.W2NmWFRINXokdA";
                       }))
                {
                    var res = await psb.SalesInvoice.QueryRecipientParty("0106:12345678", new[] { "0106:receiver" }).ConfigureAwait(false);
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
