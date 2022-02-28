using System;
using System.Security.Cryptography.X509Certificates;
using EConnect.Psb.Api;
using EConnect.Psb.Auth;
using EConnect.Psb.Client;
using EConnect.Psb.Client.Handlers;
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
        services.AddTransient<IPsbHttpMessageHandlerFactory, PsbHttpMessageHandlerFactory>();
        services.AddSingleton<IPsbAuthenticationProvider, PsbAuthenticationProvider>();

        services.AddHttpClient(nameof(PsbAuthenticationProvider))
            .ConfigurePrimaryHttpMessageHandler(provider =>
                provider
                    .GetRequiredService<IPsbHttpMessageHandlerFactory>()
                    .CreateHttpMessageHandler<IPsbAuthenticationProvider>(provider));

        services.AddHttpClient<PsbClient>()
            .ConfigurePrimaryHttpMessageHandler(provider =>
                provider
                    .GetRequiredService<IPsbHttpMessageHandlerFactory>()
                    .CreateHttpMessageHandler<PsbClient>(provider))
            .AddHttpMessageHandler(provider =>
            {
                var authenticationProvider = provider.GetRequiredService<IPsbAuthenticationProvider>();
                return new PsbAuthenticationHandler(authenticationProvider);
            });

        services.AddSingleton<IPsbMeApi, PsbMeApi>();
        services.AddSingleton<IPsbSalesInvoiceApi, PsbSalesInvoiceApi>();
        services.AddSingleton<IPsbHookApi, PsbHookApi>();

        return services;
    }
}