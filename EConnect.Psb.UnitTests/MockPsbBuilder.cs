using System;
using System.Net.Http;
using EConnect.Psb.Client.Handlers;
using Moq;

namespace EConnect.Psb.UnitTests;

public class MockPsbHttpMessageHandlerFactory : IPsbHttpMessageHandlerFactory
{
    private readonly Mock<HttpMessageHandler> _httpMock;

    public MockPsbHttpMessageHandlerFactory(Mock<HttpMessageHandler> httpMock)
    {
        _httpMock = httpMock;
    }

    public HttpMessageHandler CreateHttpMessageHandler<TClient>(IServiceProvider serviceProvider) where TClient : class
    {
        return _httpMock.Object;
    }
}