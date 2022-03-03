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
    public async Task<string> QueryRecipientParty(string senderPartyId, string[] recipientPartyIds, string? preferredDocumentTypeId = null, CancellationToken cancellation = default)
    {
        var requestUri = $"/api/v1/{HttpUtility.UrlEncode(senderPartyId)}/salesInvoice/queryRecipientParty";
        if (!string.IsNullOrEmpty(preferredDocumentTypeId))
            requestUri += "?preferredDocumentTypeId=" + HttpUtility.UrlEncode(preferredDocumentTypeId);

        var party = await _psbClient.Post<Party>(requestUri, recipientPartyIds, cancellation);
        return party.Id;
    }

    public async Task<Document> Send(string senderPartyId, FileContent file, string? receiverPartyId = null, CancellationToken cancellation = default)
    {
        var requestUri = $"/api/v1/{HttpUtility.UrlEncode(senderPartyId)}/salesInvoice/send";
        if (!string.IsNullOrEmpty(receiverPartyId))
            requestUri += "?receiverId=" + HttpUtility.UrlEncode(receiverPartyId);

        var res = await _psbClient.PostFile<Document>(requestUri, file, cancellation);
        return res;
    }

    public async Task<Document> Recognize(string senderPartyId, FileContent file, CancellationToken cancellation = default)
    {
        var requestUri = $"/api/v1/{HttpUtility.UrlEncode(senderPartyId)}/salesInvoice/recognize";

        var res = await _psbClient.PostFile<Document>(requestUri, file, cancellation);
        return res;
    }
}