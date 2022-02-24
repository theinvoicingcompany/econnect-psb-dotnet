using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EConnect.Psb.Client;
using EConnect.Psb.Models;

namespace EConnect.Psb.Api;

public class PsbMeApi : IPsbMeApi
{
    private readonly PsbClient _psbClient;

    public PsbMeApi(PsbClient psbClient)
    {
        _psbClient = psbClient;
    }

    public Task<Me> Me(CancellationToken cancellation = default)
    {
        return _psbClient.Get<Me>("/api/v1/me", cancellation);
    }
        
    public Task<IEnumerable<UserParty>> MeParties(CancellationToken cancellation = default)
    {
        return _psbClient.Get<IEnumerable<UserParty>>("/api/v1/me/party", cancellation);
    }
}