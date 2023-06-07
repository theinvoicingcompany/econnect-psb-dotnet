using System;
using EConnect.Psb.Auth;
using EConnect.Psb.Client.Handlers;
using EConnect.Psb.Config;
using EConnect.Psb.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EConnect.Psb.Hosting;

public class PsbServiceHost : PsbServiceProvider, IDisposable
{
    private readonly ServiceProvider _serviceProvider;

    private PsbServiceHost(ServiceProvider serviceProvider) : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Creates a new service provider with PsbServices. Only use this when your application doesn't have dependency injection.
    /// Otherwise we recommend to use the AddPsbService directly on your host builder.
    /// </summary>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static PsbServiceHost Create<TPsbHttpMessageHandlerFactory, TPsbAuthenticationProvider>(Action<PsbOptions> configureOptions)
        where TPsbHttpMessageHandlerFactory : class, IPsbHttpMessageHandlerFactory
        where TPsbAuthenticationProvider : class, IPsbAuthenticationProvider
    {
        var serviceProvider = new ServiceCollection()
            .AddPsbService<TPsbHttpMessageHandlerFactory, TPsbAuthenticationProvider>(configureOptions)
            .BuildServiceProvider();

        return new PsbServiceHost(serviceProvider);
    }

    /// <summary>
    /// Creates a new service provider with PsbServices. Only use this when your application doesn't have dependency injection.
    /// Otherwise we recommend to use the AddPsbService directly on your host builder.
    /// </summary>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static PsbServiceHost Create<TPsbHttpMessageHandlerFactory>(Action<PsbOptions> configureOptions)
        where TPsbHttpMessageHandlerFactory : class, IPsbHttpMessageHandlerFactory
    {
        return Create<TPsbHttpMessageHandlerFactory, PsbAuthenticationProvider>(configureOptions);
    }

    /// <summary>
    /// Creates a new service provider with PsbServices. Only use this when your application doesn't have dependency injection.
    /// Otherwise we recommend to use the AddPsbService directly on your host builder.
    /// </summary>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static PsbServiceHost Create(Action<PsbOptions> configureOptions)
    {
        return Create<PsbHttpMessageHandlerFactory>(configureOptions);
    }

    public void Dispose()
    {
        _serviceProvider.Dispose();
    }
}