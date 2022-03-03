using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using EConnect.Psb.Client;
using EConnect.Psb.Models;

namespace EConnect.Psb.Api;

public class PsbPurchaseInvoiceApi : IPsbPurchaseInvoiceApi
{
    private readonly PsbClient _psbClient;

    public PsbPurchaseInvoiceApi(PsbClient psbClient)
    {
        _psbClient = psbClient;
    }

    public async Task<string> Download(
        string partyId,
        string documentId,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var encodedDocumentId = HttpUtility.UrlEncode(documentId);
        var targetUrl = $"/api/v1/{encodedPartyId}/purchaseInvoice/{encodedDocumentId}/download";

        var file = await _psbClient.Get<Party>(targetUrl, cancellation);

        return file.Id;
    }

    public async Task<Document> Response(
        string partyId,
        string documentId,
        InvoiceResponse purchaseInvoice,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var encodedDocumentId = HttpUtility.UrlEncode(documentId);
        var targetUrl = $"/api/v1/{encodedPartyId}/purchaseInvoice/{encodedDocumentId}/response";

        var invoiceResponse = await _psbClient.Post<Document>(
            requestUri: targetUrl,
            body: purchaseInvoice,
            cancellation: cancellation
        );

        return invoiceResponse;
    }

    public async Task Delete(
        string partyId,
        string documentId,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var encodedDocumentId = HttpUtility.UrlEncode(documentId);
        var targetUrl = $"/api/v1/{encodedPartyId}/purchaseInvoice/{encodedDocumentId}";

        await _psbClient.Delete<HttpResponseMessage>(
            requestUri: targetUrl,
            cancellation: cancellation
        );
    }

    public async Task<Document> Recognize(
        string partyId,
        string file,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var targetUrl = $"/api/v1/{encodedPartyId}/purchaseInvoice/recognize";

        var document = await _psbClient.Post<Document>(
            requestUri: targetUrl,
            body: file,
            cancellation: cancellation
        );

        return document;
    }
}