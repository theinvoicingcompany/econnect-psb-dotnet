using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EConnect.Psb.AspNetCore.Mvc.UnitTests.Helpers;
using EConnect.Psb.AspNetCore.Mvc.Webhook;
using EConnect.Psb.Models;
using EConnect.Psb.WebHook;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace EConnect.Psb.AspNetCore.Mvc.UnitTests
{
    [ApiController]
    public class WebhookController : ControllerBase
    {
        // Hardcoded secret
        [PsbWebhook("secret", "/webhook")]
        public IActionResult Receiver([PsbWebhookValidSentOn] PsbWebHookEvent @event)
        {
            return Ok(@event.DocumentId);
        }
    }

    [TestClass]
    public class PsbWebhookAttributeTests : IDisposable
    {
        private readonly TestServer _testServer;
        private readonly WebApplication _app;
        private readonly HttpClient _httpClient;

        public PsbWebhookAttributeTests()
        {
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddControllers().AddApplicationPart(typeof(WebhookController).Assembly);
            builder.WebHost.UseTestServer();
            
            _app = builder.Build();
            _app.UseRouting();
            _app.MapControllers();
            _app.MapDefaultControllerRoute();

            _testServer = _app.GetTestServer();

            _app.StartAsync().GetAwaiter().GetResult();
            
            _httpClient = _app.GetTestClient();
        }

        public void Dispose()
        {
            _testServer.Dispose();
            _app.StopAsync().GetAwaiter().GetResult();
            _app.DisposeAsync().GetAwaiter().GetResult();
        }

        private Task<HttpResponseMessage> SendWebhook(string body, string signature)
        {
            return _httpClient.PostAsync("/webhook",
                new StringContent(body, Encoding.UTF8, "application/json")
                {
                    Headers = { { "X-EConnect-Signature", new[] { signature } } }
                });
        }

        [TestMethod]
        [DataRow("secret", HttpStatusCode.OK, 0, "doc1")]
        [DataRow("wrong_secret", HttpStatusCode.Unauthorized, 0, "Unauthorized")]
        [DataRow("secret", HttpStatusCode.BadRequest, -10, "PsbWebhookEvent does not fall in the expected time range. The event is too old or too new.")]
        [DataRow("secret", HttpStatusCode.BadRequest, 10, "PsbWebhookEvent does not fall in the expected time range. The event is too old or too new.")]
        public async Task ValidWebhookShouldBeOkTest(string secret, HttpStatusCode expectedStatusCode, int sentOnMin, string expectedResponseText)
        {
            var @event = PsbWebHookExamples.InvoiceSent(DateTimeOffset.UtcNow.AddMinutes(sentOnMin));
            var body = JsonConvert.SerializeObject(@event);
            var signature = PsbWebHookSignature.ComputeSignature(body, secret);

            var res = await SendWebhook(body, signature).ConfigureAwait(false);

            Assert.AreEqual(expectedStatusCode, res.StatusCode);
            Assert.IsTrue((await res.Content.ReadAsStringAsync().ConfigureAwait(false)).Contains(expectedResponseText));
        }
    }
}
