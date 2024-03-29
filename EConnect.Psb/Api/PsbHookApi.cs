﻿using System.Threading;
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

    public async Task<Hook[]> GetEnvironmentHooks(CancellationToken cancellation)
    {
        var targetUrl = $"/api/v1/hook";

        var hooks = await _psbClient.Get<Hook[]>(targetUrl, cancellation).ConfigureAwait(false);
        return hooks;
    }

    public async Task<Hook> SetEnvironmentHook(
        Hook hook,
        CancellationToken cancellation)
    {
        var targetUrl = $"/api/v1/hook";

        var createdHook = await _psbClient.Put<Hook>(
            requestUri: targetUrl,
            body: hook,
            cancellation: cancellation
        ).ConfigureAwait(false);

        return createdHook;
    }

    public async Task DeleteEnvironmentHook(
        string hookId,
        CancellationToken cancellation)
    {
        var encodedHookId = HttpUtility.UrlEncode(hookId);
        var targetUrl = $"/api/v1/hook/{encodedHookId}";

        await _psbClient.Delete(
            requestUri: targetUrl,
            cancellation: cancellation
        ).ConfigureAwait(false);
    }


    #region partyHooks

    public async Task<Hook[]> GetHooks(
        string partyId,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var targetUrl = $"/api/v1/{encodedPartyId}/hook";

        var hooks = await _psbClient.Get<Hook[]>(
            requestUri: targetUrl,
            cancellation: cancellation
        ).ConfigureAwait(false);

        return hooks;
    }

    public async Task<Hook> SetHook(
        string partyId,
        Hook hook,
        CancellationToken cancellation)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var targetUrl = $"/api/v1/{encodedPartyId}/hook";

        var createdHook = await _psbClient.Put<Hook>(
            requestUri: targetUrl,
            body: hook,
            cancellation: cancellation
        ).ConfigureAwait(false);

        return createdHook;
    }

    public async Task<string> PingHooks(
        string partyId,
        CancellationToken cancellation)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var targetUrl = $"/api/v1/{encodedPartyId}/hook/ping";

        var party = await _psbClient.Get<Party>(
            requestUri: targetUrl,
            cancellation: cancellation
        ).ConfigureAwait(false);

        return party.Id;
    }

    public async Task DeleteHook(
        string partyId,
        string hookId,
        CancellationToken cancellation)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var encodedHookId = HttpUtility.UrlEncode(hookId);
        var targetUrl = $"/api/v1/{encodedPartyId}/hook/{encodedHookId}";

        await _psbClient.Delete(
            requestUri: targetUrl,
            cancellation: cancellation
        ).ConfigureAwait(false);
    }

    #endregion

}