using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EConnect.Psb.Models;
using Moq;
using Moq.Contrib.HttpClient;
using Moq.Language.Flow;

namespace EConnect.Psb.UnitTests;

public class MockHttpMessageBuilder
{
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private ISetup<HttpMessageHandler, Task<HttpResponseMessage>>? _setup;
    private readonly string? _baseUrl;

    public MockHttpMessageBuilder(Mock<HttpMessageHandler> mockHandler, string? baseUrl)
    {
        _mockHandler = mockHandler;
        _baseUrl = baseUrl;
    }

    private static bool IsAuthenticated(HttpRequestMessage message)
    {
        return message.Headers.Authorization?.ToString() == "Bearer valid_token";
    }

    private static bool HasSubscription(HttpRequestMessage message)
    {
        return message.Headers.TryGetValues("Subscription-Key", out var keys) && keys.Any();
    }

    public static bool HasMultipartFormDataContent<TType>(HttpRequestMessage message)
    {
        return message.Content is MultipartFormDataContent multipart &&
               multipart.FirstOrDefault(m => m is TType) != default;
    }

    private static bool HasEConnectDocumentId(HttpRequestMessage message)
    {
        return message.Headers.TryGetValues("X-EConnect-DocumentId", out var keys) && keys.Any();
    }

    private static bool HasEConnectDomainId(HttpRequestMessage message)
    {
        return message.Headers.TryGetValues("X-EConnect-DomainId", out var keys) && keys.Any();
    }

    private HttpResponseMessage Xml(HttpStatusCode status, string xml) => new(status)
    {
        Content = new StringContent(xml, Encoding.UTF8, "application/xml")
    };

    private HttpResponseMessage Json(HttpStatusCode status, string json) => new(status)
    {
        Content = new StringContent(json, Encoding.UTF8, "application/json")
    };

    private HttpResponseMessage Plain(HttpStatusCode status, string text) => new(status)
    {
        Content = new StringContent(text, Encoding.UTF8)
    };

    public MockHttpMessageBuilder SetupFormPost(string path)
    {
        _setup = _mockHandler.SetupRequest(HttpMethod.Post, _baseUrl + path);//, message => message.Content.Headers.ContentType == MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded"));
        return this;
    }

    public MockHttpMessageBuilder Setup(HttpMethod method, string path,
        bool ensureAuthorizationHeader = true,
        bool ensureSubscriptionHeader = true,
        bool ensureFileUpload = false,
        bool ensureMetaAttributes = false,
        bool ensureEConnectDocumentId = false,
        bool ensureEConnectDomainId = false)
    {
        return Setup(method, path, message =>
        {
            var auth = !ensureAuthorizationHeader || IsAuthenticated(message);
            var sub = !ensureSubscriptionHeader || HasSubscription(message);
            var file = !ensureFileUpload || HasMultipartFormDataContent<StreamContent>(message);
            var meta = !ensureMetaAttributes || HasMultipartFormDataContent<StringContent>(message);
            var docId = !ensureEConnectDocumentId || HasEConnectDocumentId(message);
            var domain = !ensureEConnectDomainId || HasEConnectDomainId(message);

            return auth && sub && file && meta && docId && domain;
        });
    }

    public MockHttpMessageBuilder Setup(HttpMethod method, string path, Predicate<HttpRequestMessage> match)
    {
        _setup = _mockHandler.SetupRequest(method, _baseUrl + path, match);
        return this;
    }

    public MockHttpMessageBuilder Result(string body, HttpStatusCode statusCode = HttpStatusCode.OK, string contentType = "json")
    {
        if (_setup == null)
            throw new Exception("Please call the 'Setup' before this 'Result' call.");

        var ret = contentType switch
        {
            "json" => Json(statusCode, body),
            "xml" => Xml(statusCode, body),
            _ => Plain(statusCode, body)
        };

        _setup.ReturnsAsync(ret).Verifiable();
        return this;
    }
}