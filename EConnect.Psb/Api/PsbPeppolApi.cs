﻿using System.Threading;
using System.Threading.Tasks;
using System.Web;
using EConnect.Psb.Client;
using EConnect.Psb.Models;
using EConnect.Psb.Models.Peppol;

namespace EConnect.Psb.Api;

public class PsbPeppolApi : PsbPeppolLookupApi, IPsbPeppolApi
{
    private readonly PsbClient _psbClient;

    public PsbPeppolApi(PsbClient psbClient)
    {
        _psbClient = psbClient;
    }

    protected override async Task<DeliveryOption[]> GetDeliveryOptions(
       string requestUri,
       string? domainId,
       CancellationToken cancellation)
    {
        var deliveryOptions = await _psbClient.Get<DeliveryOption[]>(requestUri, domainId, cancellation).ConfigureAwait(false);

        return deliveryOptions;
    }

    public async Task<PeppolConfig> GetEnvironmentConfig(string? domainId = null, CancellationToken cancellation = default)
    {
        var targetUrl = "/api/v1/peppol/config";

        var peppolPartyConfig = await _psbClient.Get<PeppolConfig>(targetUrl, domainId, cancellation).ConfigureAwait(false);

        return peppolPartyConfig;
    }

    public async Task<PeppolConfig> PutEnvironmentConfig(
        PeppolConfig config,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var targetUrl = "/api/v1/peppol/config";

        var peppolPartyConfig = await _psbClient.Put<PeppolConfig>(targetUrl, config, domainId, cancellation).ConfigureAwait(false);

        return peppolPartyConfig;
    }

    public async Task<ListResult<Party>> GetParties(string? domainId = null, CancellationToken cancellation = default)
    {
        var targetUrl = "/api/v1/peppol/config/party";

        var partyPagedResult = await _psbClient.Get<ListResult<Party>>(targetUrl, domainId, cancellation).ConfigureAwait(false);
        return partyPagedResult;
    }

    public async Task<PeppolConfig> GetConfig(
        string partyId,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var targetUrl = $"/api/v1/peppol/config/party/{encodedPartyId}";

        var res = await _psbClient.Get<PeppolConfig>(targetUrl, domainId, cancellation).ConfigureAwait(false);
        return res;
    }

    public async Task<PeppolConfig> PutConfig(
        string partyId,
        PeppolConfig config,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var targetUrl = $"/api/v1/peppol/config/party/{encodedPartyId}";

        var res = await _psbClient.Put<PeppolConfig>(targetUrl, config, domainId, cancellation).ConfigureAwait(false);
        return res;
    }

    public async Task DeleteConfig(
        string partyId,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var targetUrl = $"/api/v1/peppol/config/party/{encodedPartyId}";

        await _psbClient.Delete(targetUrl, domainId, cancellation).ConfigureAwait(false);
    }
}
