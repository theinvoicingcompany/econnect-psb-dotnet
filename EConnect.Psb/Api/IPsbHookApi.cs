using EConnect.Psb.Models;
using System.Threading;
using System.Threading.Tasks;

namespace EConnect.Psb.Api;

public interface IPsbHookApi
{
    Task<Hook[]> GetEnvironmentHooks(CancellationToken cancellation = default);

    Task<Hook> SetEnvironmentHook(Hook hook, CancellationToken cancellation = default);

    Task DeleteEnvironmentHook(string hookId, CancellationToken cancellation = default);

    Task<Hook[]> GetPartyHooks(string partyId, CancellationToken cancellation = default);

    Task<Hook> SetPartyHook(string partyId, Hook hook, CancellationToken cancellation = default);

    Task<string> PingPartyHooks(string partyId, CancellationToken cancellation = default);

    Task DeletePartyHook(string hookId, string partyId, CancellationToken cancellation = default);
}