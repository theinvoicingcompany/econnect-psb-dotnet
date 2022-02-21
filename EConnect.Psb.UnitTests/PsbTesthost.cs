using System;
using System.Threading;
using System.Threading.Tasks;
using EConnect.Psb.Api;
using EConnect.Psb.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EConnect.Psb.UnitTests;

public abstract class PsbTestContext : IDisposable
{
    private readonly IHost _host;

    protected PsbTestContext()
    {
        var hostBuilder = new HostBuilder()
            .ConfigureServices(services =>
            {
                services.AddPsbService(_ =>
                {
                    _.Username = "";
                    _.Password = "";
                    _.PsbUrl = "https://accp-psb.econnect.eu";
                    _.IdentityUrl = "https://accp-identity.econnect.eu";
                    _.ClientId = "2210f77eed3a4ab2";
                    _.ClientSecret = "ddded83702534a6c9cadde3d1bf3e94a";
                    _.SubscriptionKey = "Sandbox.Accp.W2NmWFRINXokdA";
                });
            });

        _host = hostBuilder
            .Build();

        _host.Start();
    }

    public IPsbMeApi MeApi => _host.Services.GetRequiredService<IPsbMeApi>();

    public void Dispose()
    {
        _host.StopAsync();
        _host.Dispose();
    }
}