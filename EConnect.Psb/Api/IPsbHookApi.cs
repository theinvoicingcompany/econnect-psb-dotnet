using EConnect.Psb.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EConnect.Psb.Api;

public interface IPsbHookApi
{
    public Task<Hook[]> GetEnviromentHooks(CancellationToken cancellation = default);

    Task<Hook> SetEnviromentHook(Hook hook, CancellationToken cancellation = default);
    
    Task DeleteDefaultHook(string hookId, CancellationToken cancellation = default);
    
    Task<Hook[]> GetPartyHooks(string partyId, CancellationToken cancellation = default);
    
    Task<Hook> SetPartyHooks(string partyId, Hook hook, CancellationToken cancellation = default);
    
    Task<string> PingPartyHooks(string partyId, CancellationToken cancellation = default);
    
    Task DeletePartyHook(string hookId, string partyId, CancellationToken cancellation = default);
}