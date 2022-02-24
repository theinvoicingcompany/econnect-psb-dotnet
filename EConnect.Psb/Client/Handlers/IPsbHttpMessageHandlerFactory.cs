using System;
using System.Net.Http;

namespace EConnect.Psb.Client.Handlers;

public interface IPsbHttpMessageHandlerFactory
{
    HttpMessageHandler CreateHttpMessageHandler<TClient>(IServiceProvider serviceProvider) where TClient : class;
}