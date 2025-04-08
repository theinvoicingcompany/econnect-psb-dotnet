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

    public Task<Me> Me(string? domainId = null, CancellationToken cancellation = default)
    {
        return _psbClient.Get<Me>("/api/v1/me", domainId, cancellation);
    }

    public Task<UserParty[]> MeParties(string? domainId = null, CancellationToken cancellation = default)
    {
        return _psbClient.Get<UserParty[]>("/api/v1/me/party", domainId, cancellation);
    }
}