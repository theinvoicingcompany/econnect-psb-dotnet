using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using EConnect.Psb.Client;
using EConnect.Psb.Models;

namespace EConnect.Psb.Api;

public class PsbPurchaseOrderApi : IPsbPurchaseOrderApi
{
    private readonly PsbClient _psbClient;

    public PsbPurchaseOrderApi(PsbClient psbClient)
    {
        _psbClient = psbClient;
    }

    public async Task<Party> QueryRecipientParty(
        string senderPartyId,
        IEnumerable<string> recipientPartyIds,
        string? preferredDocumentTypeId = null,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(senderPartyId);
        var requestUri = $"/api/v1/{encodedPartyId}/purchaseOrder/queryRecipientParty";

        if (!string.IsNullOrEmpty(preferredDocumentTypeId))
            requestUri += "?preferredDocumentTypeId=" + HttpUtility.UrlEncode(preferredDocumentTypeId);

        var party = await _psbClient.Post<Party>(requestUri, recipientPartyIds, cancellation: cancellation).ConfigureAwait(false);

        return party;
    }

    public async Task<Document> Send(
        string partyId,
        FileContent file,
        string? receiverPartyId = null,
        string? channel = null,
        string? documentId = null,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var requestUri = $"/api/v1/{encodedPartyId}/purchaseOrder/send";

        var query = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(receiverPartyId))
            query.Add("receiverId", receiverPartyId!);

        if (!string.IsNullOrEmpty(channel))
            query.Add("channel", channel!);

        var queryString = await new FormUrlEncodedContent(query).ReadAsStringAsync().ConfigureAwait(false);

        if (!string.IsNullOrEmpty(queryString))
            requestUri += "?" + queryString;

        var res = await _psbClient.PostFile<Document>(
            requestUri, 
            file, 
            documentId,
            cancellation).ConfigureAwait(false);
        return res;
    }

    public async Task<Document> Cancel(
        string partyId,
        string documentId, 
        OrderCancellation message,
        string? cancelDocumentId = null,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var encodedDocumentId = HttpUtility.UrlEncode(documentId);
        var requestUri = $"/api/v1/{encodedPartyId}/purchaseOrder/{encodedDocumentId}/cancel";
        
        var res = await _psbClient.Post<Document>(
                requestUri,
                message,
                cancelDocumentId,
                cancellation).ConfigureAwait(false);

        return res;
    }
}