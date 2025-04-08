using System.Threading;
using System.Threading.Tasks;
using System.Web;
using EConnect.Psb.Client;
using EConnect.Psb.Models;

namespace EConnect.Psb.Api;

public class PsbUserApi : IPsbUserApi
{
    private readonly PsbClient _psbClient;

    public PsbUserApi(PsbClient psbClient)
    {
        _psbClient = psbClient;
    }

    public async Task<User[]> GetUsers(string? domainId = null, CancellationToken cancellation = default)
    {
        var requestUri = "/api/v1/user";

        var res = await _psbClient.Get<User[]>(requestUri, domainId, cancellation);

        return res;
    }

    public async Task<User> SetUser(User user, string? domainId = null, CancellationToken cancellation = default)
    {
        var requestUri = "/api/v1/user";

        var res = await _psbClient.Put<User>(requestUri, user, domainId, cancellation);

        return res;
    }

    public Task DeleteUser(string name, string? domainId = null, CancellationToken cancellation = default)
    {
        var encodedName = HttpUtility.UrlEncode(name);

        var requestUri = $"/api/v1/user/{encodedName}";

        return _psbClient.Delete(requestUri, domainId, cancellation);
    }

    public async Task<UserParty[]> GetUserParties(string name, string? domainId = null, CancellationToken cancellation = default)
    {
        var encodedUsername = HttpUtility.UrlEncode(name);

        var requestUri = $"/api/v1/user/{encodedUsername}/party";

        var res = await _psbClient.Get<UserParty[]>(requestUri, domainId, cancellation);

        return res;
    }

    public async Task<UserParty> SetUserParty(string name, UserParty userParties, string? domainId = null, CancellationToken cancellation = default)
    {
        var encodedUsername = HttpUtility.UrlEncode(name);

        var requestUri = $"/api/v1/user/{encodedUsername}/party";

        var res = await _psbClient.Put<UserParty>(requestUri, userParties, domainId, cancellation);

        return res;
    }

    public Task DeleteUserParty(string name, string partyId, string? domainId = null, CancellationToken cancellation = default)
    {
        var encodedUsername = HttpUtility.UrlEncode(name);
        var encodedPartyId = HttpUtility.UrlEncode(partyId);

        var requestUri = $"/api/v1/user/{encodedUsername}/party/{encodedPartyId}";

        return _psbClient.Delete(requestUri, domainId, cancellation);
    }
}