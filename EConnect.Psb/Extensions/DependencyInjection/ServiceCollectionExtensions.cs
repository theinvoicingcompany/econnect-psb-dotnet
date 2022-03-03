using System;
using EConnect.Psb.Api;
using EConnect.Psb.Auth;
using EConnect.Psb.Client;
using EConnect.Psb.Client.Handlers;
using EConnect.Psb.Config;
using Microsoft.Extensions.DependencyInjection;

namespace EConnect.Psb.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPsbHttpClients<TPsbHttpMessageHandlerFactory>(this IServiceCollection services)
        where TPsbHttpMessageHandlerFactory : class, IPsbHttpMessageHandlerFactory
    {
        services.AddTransient<IPsbHttpMessageHandlerFactory, TPsbHttpMessageHandlerFactory>();

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

        return services;
    }

    public static IServiceCollection AddPsbApis(this IServiceCollection services)
    {
        services.AddSingleton<IPsbMeApi, PsbMeApi>();
        services.AddSingleton<IPsbSalesInvoiceApi, PsbSalesInvoiceApi>();
        services.AddSingleton<IPsbHookApi, PsbHookApi>();
        services.AddSingleton<IPsbPurchaseInvoiceApi, PsbPurchaseInvoiceApi>();

        return services;
    }

    public static IServiceCollection AddPsbService<TPsbHttpMessageHandlerFactory, TPsbAuthenticationProvider>(
        this IServiceCollection services,
        Action<PsbOptions> configureOptions)
        where TPsbHttpMessageHandlerFactory : class, IPsbHttpMessageHandlerFactory
        where TPsbAuthenticationProvider : class, IPsbAuthenticationProvider
    {
        services.Configure(configureOptions);
        services.AddSingleton<IPsbAuthenticationProvider, TPsbAuthenticationProvider>();

        services.AddPsbHttpClients<TPsbHttpMessageHandlerFactory>();
        services.AddPsbApis();

        return services;
    }

    public static IServiceCollection AddPsbService<TPsbHttpMessageHandlerFactory>(
        this IServiceCollection services,
        Action<PsbOptions> configureOptions)
        where TPsbHttpMessageHandlerFactory : class, IPsbHttpMessageHandlerFactory
    {
        return services.AddPsbService<TPsbHttpMessageHandlerFactory, PsbAuthenticationProvider>(configureOptions);
    }
    
    public static IServiceCollection AddPsbService(
        this IServiceCollection services,
        Action<PsbOptions> configureOptions)
    {
        return services.AddPsbService<PsbHttpMessageHandlerFactory>(configureOptions);
    }

}
