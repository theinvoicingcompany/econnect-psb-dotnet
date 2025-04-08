using System.Threading;
using System.Threading.Tasks;
using System.Web;
using EConnect.Psb.Client;
using EConnect.Psb.Models;

namespace EConnect.Psb.Api;

public class PsbHookApi : IPsbHookApi
{
    private readonly PsbClient _psbClient;

    public PsbHookApi(PsbClient psbClient)
    {
        _psbClient = psbClient;
    }

    public async Task<Hook[]> GetEnvironmentHooks(string? domainId = null, CancellationToken cancellation = default)
    {
        var targetUrl = $"/api/v1/hook";

        var hooks = await _psbClient.Get<Hook[]>(targetUrl, domainId, cancellation).ConfigureAwait(false);
        return hooks;
    }

    public async Task<Hook> SetEnvironmentHook(
        Hook hook,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var targetUrl = $"/api/v1/hook";

        var createdHook = await _psbClient.Put<Hook>(
            targetUrl,
            hook,
            domainId,
            cancellation
        ).ConfigureAwait(false);

        return createdHook;
    }

    public async Task DeleteEnvironmentHook(
        string hookId,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var encodedHookId = HttpUtility.UrlEncode(hookId);
        var targetUrl = $"/api/v1/hook/{encodedHookId}";

        await _psbClient.Delete(
            targetUrl,
            domainId,
            cancellation
        ).ConfigureAwait(false);
    }


    #region partyHooks

    public async Task<Hook[]> GetHooks(
        string partyId,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var targetUrl = $"/api/v1/{encodedPartyId}/hook";

        var hooks = await _psbClient.Get<Hook[]>(
            targetUrl,
            domainId,
            cancellation
        ).ConfigureAwait(false);

        return hooks;
    }

    public async Task<Hook> SetHook(
        string partyId,
        Hook hook,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var targetUrl = $"/api/v1/{encodedPartyId}/hook";

        var createdHook = await _psbClient.Put<Hook>(
            targetUrl,
            hook,
            domainId,
             cancellation
        ).ConfigureAwait(false);

        return createdHook;
    }

    public async Task<string> PingHooks(
        string partyId,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var targetUrl = $"/api/v1/{encodedPartyId}/hook/ping";

        var party = await _psbClient.Get<Party>(
            targetUrl,
            domainId,
            cancellation
        ).ConfigureAwait(false);

        return party.Id;
    }

    public async Task DeleteHook(
        string partyId,
        string hookId,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var encodedHookId = HttpUtility.UrlEncode(hookId);
        var targetUrl = $"/api/v1/{encodedPartyId}/hook/{encodedHookId}";

        await _psbClient.Delete(
            targetUrl,
            domainId,
            cancellation
        ).ConfigureAwait(false);
    }

    #endregion

}