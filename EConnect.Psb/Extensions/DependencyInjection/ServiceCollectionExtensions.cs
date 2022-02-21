using System;
using System.Net.Http;
using System.Security.Authentication;
using EConnect.Psb.Api;
using EConnect.Psb.Auth;
using EConnect.Psb.Client;
using EConnect.Psb.Config;
using Microsoft.Extensions.DependencyInjection;

namespace EConnect.Psb.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPsbService(
        this IServiceCollection services,
        Action<PsbOptions> configureOptions)
    {
        services.Configure(configureOptions);

        services.AddHttpClient<IPsbAuthenticationProvider, PsbAuthenticationProvider>()
            .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
            {
                AllowAutoRedirect = false,
                UseDefaultCredentials = false,
                UseCookies = false,
                SslProtocols = SslProtocols.Tls12
            });

        services.AddHttpClient<PsbClient>()
            .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
            {
                AllowAutoRedirect = false,
                UseDefaultCredentials = false,
                UseCookies = false,
                SslProtocols = SslProtocols.Tls12
            }).AddHttpMessageHandler(provider =>
            {
                var authenticationProvider = provider.GetRequiredService<IPsbAuthenticationProvider>();
                return new PsbAuthenticationHandler(authenticationProvider);
            });

        services.AddSingleton<IPsbMeApi, PsbMeApi>();

        return services;
    }
}