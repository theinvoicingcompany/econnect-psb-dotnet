using System.Threading;
using System.Threading.Tasks;
using System.Web;
using EConnect.Psb.Client;
using EConnect.Psb.Models;

namespace EConnect.Psb.Api;

public class PsbSalesOrderApi : IPsbSalesOrderApi
{
    private readonly PsbClient _psbClient;

    public PsbSalesOrderApi(PsbClient psbClient)
    {
        _psbClient = psbClient;
    }

    public async Task<FileContent> Download(
        string partyId,
        string documentId,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var encodedDocumentId = HttpUtility.UrlEncode(documentId);

        var targetUrl = $"/api/v1/{encodedPartyId}/salesOrder/{encodedDocumentId}/download";

        var file = await _psbClient.Get<FileContent>(
            targetUrl,
            domainId,
            cancellation).ConfigureAwait(false);

        return file;
    }

    public async Task Delete(
        string partyId,
        string documentId,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var encodedDocumentId = HttpUtility.UrlEncode(documentId);
        var targetUrl = $"/api/v1/{encodedPartyId}/salesOrder/{encodedDocumentId}";

        await _psbClient.Delete(
            targetUrl,
            domainId,
            cancellation).ConfigureAwait(false);
    }

    public async Task<Document> Response(
        string partyId,
        string documentId,
        OrderResponse orderResponse,
        string? responseDocumentId = null,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var encodedDocumentId = HttpUtility.UrlEncode(documentId);
        var targetUrl = $"/api/v1/{encodedPartyId}/salesOrder/{encodedDocumentId}/response";

        var res = await _psbClient.Post<Document>(
            targetUrl,
            orderResponse,
            responseDocumentId,
            domainId,
            cancellation).ConfigureAwait(false);

        return res;
    }
}