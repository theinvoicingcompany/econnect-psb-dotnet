using System;
using System.Net.Http;
using EConnect.Psb.Config;
using EConnect.Psb.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Contrib.HttpClient;

namespace EConnect.Psb.UnitTests;

public abstract class PsbTestContext : IDisposable
{
    private readonly IHost _host;
    private readonly Mock<HttpMessageHandler> _httpMock;

    protected PsbTestContext()
    {
        var hostBuilder = new HostBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<Mock<HttpMessageHandler>>();
                services.AddPsbService<MockPsbHttpMessageHandlerFactory>(_ =>
                {
                    _.Username = "user";
                    _.Password = "pass";
                    _.PsbUrl = "https://psb.econnect.mock";
                    _.IdentityUrl = "https://identity.econnect.mock";
                    _.ClientId = "client_id";
                    _.ClientSecret = "client_secret";
                    _.SubscriptionKey = "Subscription.Mock";
                });
            });

        _host = hostBuilder
            .Build();

        _host.Start();
        _httpMock = GetRequiredService<Mock<HttpMessageHandler>>();
    }

    public TService GetRequiredService<TService>() where TService : class
    {
        return _host.Services.GetRequiredService<TService>();
    }
    
    public void Configure(string? baseUrl, Action<MockHttpMessageBuilder> builder)
    {
        builder(new MockHttpMessageBuilder(_httpMock, baseUrl));
    }

    public void SetAccessToken(string token = "valid_token", int expiresIn = 3600)
    {
        var baseUrl = _host.Services.GetRequiredService<IOptions<PsbOptions>>().Value.IdentityUrl;

        Configure(baseUrl, builder =>
        {
            var json = @"{""access_token"":""" + token + @""",""expires_in"":" + expiresIn + @",""token_type"":""Bearer""}";
            builder
                .SetupFormPost("/connect/token")
                .Result(json);
        });
    }

    public void Configure(Action<MockHttpMessageBuilder> builder)
    {
        var baseUrl = _host.Services.GetRequiredService<IOptions<PsbOptions>>().Value.PsbUrl;
        Configure(baseUrl, builder);
    }

    public void VerifyAnyRequest(Times? times = null,
        string? failMessage = null)
    {
        _httpMock.VerifyAnyRequest(times, failMessage);
    }

    public void Dispose()
    {
        _host.StopAsync();
        _host.Dispose();
    }
}
