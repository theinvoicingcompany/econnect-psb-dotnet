using EConnect.Psb.Models;
using System.Threading;
using System.Threading.Tasks;

namespace EConnect.Psb.Api;

public interface IPsbHookApi
{
    Task<Hook[]> GetEnvironmentHooks(CancellationToken cancellation = default);

    Task<Hook> SetEnvironmentHook(Hook hook, CancellationToken cancellation = default);

    Task DeleteEnvironmentHook(string hookId, CancellationToken cancellation = default);

    Task<Hook[]> GetHooks(string partyId, CancellationToken cancellation = default);

    Task<Hook> SetHook(string partyId, Hook hook, CancellationToken cancellation = default);

    Task<string> PingHooks(string partyId, CancellationToken cancellation = default);

    Task DeleteHook(string partyId, string hookId, CancellationToken cancellation = default);
}