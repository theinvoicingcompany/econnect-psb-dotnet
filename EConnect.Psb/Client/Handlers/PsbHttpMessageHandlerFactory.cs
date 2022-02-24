using System;
using System.Net.Http;
using System.Security.Authentication;

namespace EConnect.Psb.Client.Handlers;

public class PsbHttpMessageHandlerFactory : IPsbHttpMessageHandlerFactory
{
    public HttpMessageHandler CreateHttpMessageHandler<TClient>(IServiceProvider serviceProvider) where TClient : class
    {
        return new HttpClientHandler
        {
            AllowAutoRedirect = false,
            UseDefaultCredentials = false,
            UseCookies = false,
            SslProtocols = SslProtocols.Tls12
        };
    }
}