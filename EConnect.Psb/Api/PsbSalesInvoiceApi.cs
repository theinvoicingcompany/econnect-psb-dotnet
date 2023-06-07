using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using EConnect.Psb.Client;
using EConnect.Psb.Models;

namespace EConnect.Psb.Api;

public class PsbSalesInvoiceApi : IPsbSalesInvoiceApi
{
    private readonly PsbClient _psbClient;

    public PsbSalesInvoiceApi(PsbClient psbClient)
    {
        _psbClient = psbClient;
    }
    public async Task<string> QueryRecipientParty(string senderPartyId, IEnumerable<string> recipientPartyIds, string? preferredDocumentTypeId = null, CancellationToken cancellation = default)
    {
        var requestUri = $"/api/v1/{HttpUtility.UrlEncode(senderPartyId)}/salesInvoice/queryRecipientParty";
        if (!string.IsNullOrEmpty(preferredDocumentTypeId))
            requestUri += "?preferredDocumentTypeId=" + HttpUtility.UrlEncode(preferredDocumentTypeId);

        var party = await _psbClient.Post<Party>(requestUri, recipientPartyIds, cancellation: cancellation).ConfigureAwait(false);
        return party.Id;
    }

    public async Task<Document> Send(
        string senderPartyId,
        FileContent file,
        string? receiverPartyId = null,
        string? channel = null,
        string? documentId = null,
        CancellationToken cancellation = default)
    {
        var requestUri = $"/api/v1/{HttpUtility.UrlEncode(senderPartyId)}/salesInvoice/send";

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

    public async Task<Document> Recognize(
        string senderPartyId,
        FileContent file,
        string? channel = null,
        string? documentId = null,
        CancellationToken cancellation = default)
    {
        var requestUri = $"/api/v1/{HttpUtility.UrlEncode(senderPartyId)}/salesInvoice/recognize";

        if (!string.IsNullOrEmpty(channel))
            requestUri += "?channel=" + HttpUtility.UrlEncode(channel);

        var res = await _psbClient.PostFile<Document>(
            requestUri,
            file,
            documentId,
            cancellation).ConfigureAwait(false);
        
        return res;
    }
}