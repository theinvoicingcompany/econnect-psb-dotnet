using EConnect.Psb.Models;
using System.Threading;
using System.Threading.Tasks;

namespace EConnect.Psb.Api;

public interface IPsbHookApi
{
    Task<Hook[]> GetEnvironmentHooks(string? domainId = null, CancellationToken cancellation = default);

    Task<Hook> SetEnvironmentHook(Hook hook, string? domainId = null, CancellationToken cancellation = default);

    Task DeleteEnvironmentHook(string hookId, string? domainId = null, CancellationToken cancellation = default);

    Task<Hook[]> GetHooks(string partyId, string? domainId = null, CancellationToken cancellation = default);

    Task<Hook> SetHook(string partyId, Hook hook, string? domainId = null, CancellationToken cancellation = default);

    Task<string> PingHooks(string partyId, string? domainId = null, CancellationToken cancellation = default);

    Task DeleteHook(string partyId, string hookId, string? domainId = null, CancellationToken cancellation = default);
}