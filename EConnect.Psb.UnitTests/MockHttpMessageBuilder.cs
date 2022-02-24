using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Moq.Contrib.HttpClient;
using Moq.Language.Flow;

namespace EConnect.Psb.UnitTests;

public class MockHttpMessageBuilder
{
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private ISetup<HttpMessageHandler, Task<HttpResponseMessage>>? _setup;
    private readonly string _baseUrl;

    public MockHttpMessageBuilder(Mock<HttpMessageHandler> mockHandler, string baseUrl)
    {
        _mockHandler = mockHandler;
        _baseUrl = baseUrl;
    }

    private bool IsAuthenticated(HttpRequestMessage message)
    {
        return message.Headers.Authorization?.ToString() == "Bearer valid_token";
    }

    private bool HasSubscription(HttpRequestMessage message)
    {
        return message.Headers.TryGetValues("Subscription-Key", out var keys) && keys.Any();
    }

    private HttpResponseMessage Json(HttpStatusCode status, string json) => new(status)
    {
        Content = new StringContent(json, Encoding.UTF8, "application/json")
    };

    public MockHttpMessageBuilder SetupFormPost(string path)
    {
        _setup = _mockHandler.SetupRequest(HttpMethod.Post, _baseUrl + path);//, message => message.Content.Headers.ContentType == MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded"));
        return this;
    }

    public MockHttpMessageBuilder Setup(HttpMethod method, string path, bool ensureAuthorizationHeader = true, bool ensureSubscriptionHeader = true)
    {
        return Setup(method, path, message =>
        {
            var auth = !ensureAuthorizationHeader || IsAuthenticated(message);
            var sub = !ensureSubscriptionHeader || HasSubscription(message);
            return auth && sub;
        });
    }

    public MockHttpMessageBuilder Setup(HttpMethod method, string path, Predicate<HttpRequestMessage> match)
    {
        _setup = _mockHandler.SetupRequest(method, _baseUrl + path, match);
        return this;
    }

    public MockHttpMessageBuilder Result(string json, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        if (_setup == null)
            throw new Exception("Please call the 'Setup' before this 'Result' call.");

        _setup.ReturnsAsync(Json(statusCode, json)).Verifiable();
        return this;
    }
}