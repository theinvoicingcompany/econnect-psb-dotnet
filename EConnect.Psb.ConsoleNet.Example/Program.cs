using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EConnect.Psb.Hosting;
using EConnect.Psb.Models;

namespace EConnect.Psb.ConsoleNet.Example
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            const string psbUrl = "https://accp-psb.econnect.eu";
            const string identityUrl = "https://accp-identity.econnect.eu";
            const string subscriptionKey = "eConnectInternalApaasSubscription";
            const string username = "flowtestuser1";
            const string password = "eConnect#!12";
            const string clientId = "8deb9b756f2f4620";
            const string clientSecret = "z8461xo2rpi5oyuzk6ore5dwtpog07bliuft";

            using var psb = PsbServiceHost.Create(_ =>
            {
                _.PsbUrl = psbUrl;
                _.IdentityUrl = identityUrl;
                _.SubscriptionKey = subscriptionKey;
                _.Username = username;
                _.Password = password;
                _.ClientId = clientId;
                _.ClientSecret = clientSecret;
            });

            // Specify the path to the folder containing the files
            const string folderPath = @"C:\Users\JochemVanWageningen\OneDrive - eVerbinding\Desktop\nuget test";

            try
            {
                // Get all files in the folder
                var files = Directory.GetFiles(folderPath);

                var count = 0;

                // Loop through each file
                foreach (var filePath in files.Where(s => s.EndsWith(".xml")))
                {
                    count++;
                    // Extract information from the filename
                    var fileName = Path.GetFileNameWithoutExtension(filePath);

                    var docId = fileName.Split('_');
                    //  var metaData = File.ReadAllText($"{folderPath}/42_{docId[1]}_0106%3A24135941_meta.json");

                    // Read the file content
                    var content = new FileContent(File.ReadAllBytes(filePath), fileName);

                    Console.WriteLine(count);
                    Console.WriteLine(fileName);

                    // Send the file using GenericApi.Send
                    // Be aware that the correct Hook is available in the account
                    var response = await psb.GenericApi.Receive(
                        "0106:flowtestuser1p2",
                        content,
                        "ReceiveInvoice",
                        metaAttributes: new Dictionary<string, string>() { { "test1", "value1" }, { "test2", "value2" } });

                    //// Process the response as needed
                    Console.WriteLine($"File {filePath} sent. Response Code: {response.Id}. Count: {count}");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
