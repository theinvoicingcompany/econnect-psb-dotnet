using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace EConnect.Psb.UnitTests.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection RemoveImplementationType<TService>(this IServiceCollection services)
    {
        var impl = services.First(s => s.ServiceType == typeof(TService));
        services.Remove(impl);
        return services;
    }
}