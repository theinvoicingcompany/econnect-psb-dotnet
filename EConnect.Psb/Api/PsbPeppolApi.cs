using EConnect.Psb.Client;
using EConnect.Psb.Models;
using EConnect.Psb.Models.Peppol;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace EConnect.Psb.Api;

public class PsbPeppolApi : IPsbPeppolApi
{
    private readonly PsbClient _psbClient;

    public PsbPeppolApi(PsbClient psbClient)
    {
        _psbClient = psbClient;
    }

    public async Task<DeliveryOption[]> GetDeliveryOption(CancellationToken cancellation = default)
    {
        var targetUrl = $"/api/v1/peppol/deliveryOption";

        var deliveryOptions = await _psbClient.Get<DeliveryOption[]>(targetUrl, cancellation);
        
        return deliveryOptions;
    }

    public async Task<PeppolPartyConfig> GetEnviromentConfig(CancellationToken cancellation = default)
    {
        var targetUrl = $"/api/v1/peppol/config";

        var peppolPartyConfig = await _psbClient.Get<PeppolPartyConfig>(targetUrl, cancellation);

        return peppolPartyConfig;
    }

    public async Task<PeppolPartyConfig> PutEnviromentConfig(
        PeppolPartyConfig partyConfig, 
        CancellationToken cancellation = default)
    {
        var targetUrl = $"/api/v1/peppol/config";

        var peppolPartyConfig = await _psbClient.Put<PeppolPartyConfig>(targetUrl, cancellation);

        return peppolPartyConfig;
    }

    public async Task<Party[]> GetParties(CancellationToken cancellation = default)
    {
        var targetUrl = $"/api/v1/peppol/config/party";

        var partyPagedResult = await _psbClient.Get<Party[]>(targetUrl, cancellation);

        return partyPagedResult;
    }

    public async Task<PeppolPartyConfig> GetPartyConfig(
        string partyId,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var targetUrl = $"/api/v1/peppol/config/party/{encodedPartyId}";

        var partyPagedResult = await _psbClient.Get<PeppolPartyConfig>(
            requestUri: targetUrl,
            cancellation: cancellation
        );

        return partyPagedResult;
    }

    public async Task<PeppolPartyConfig> PutPartyConfig(
        string partyId,
        PeppolPartyConfig partyConfig,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var targetUrl = $"/api/v1/peppol/config/party/{encodedPartyId}";

        var partyPagedResult = await _psbClient.Put<PeppolPartyConfig>(
            requestUri: targetUrl,
            body: partyConfig,
            cancellation: cancellation
        );
        
        return partyPagedResult;
    }

    public async Task DeletePartyConfig(
        string partyId,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var targetUrl = $"/api/v1/peppol/config/party/{encodedPartyId}";

        await _psbClient.Delete(
            requestUri: targetUrl,
            cancellation: cancellation
        );
    }
}
