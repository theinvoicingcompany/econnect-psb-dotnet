﻿using System.Threading;
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
        string partyId,
        string[] recipientPartyIds,
        string? preferredDocumentTypeId = null,
        CancellationToken cancellation = default)
    {
        var encodedpartyId = HttpUtility.UrlEncode(partyId);
        var requestUri = $"/api/v1/{encodedpartyId}/purchaseOrder/queryRecipientParty";

        if (!string.IsNullOrEmpty(preferredDocumentTypeId))
            requestUri += "?preferredDocumentTypeId=" + HttpUtility.UrlEncode(preferredDocumentTypeId);

        var party = await _psbClient.Post<Party>(requestUri, recipientPartyIds, cancellation);

        return party;
    }

    public async Task<Document> Send(
        string partyId,
        FileContent file,
        string? receiverPartyId = null,
        CancellationToken cancellation = default)
    {
        var encodedpartyId = HttpUtility.UrlEncode(partyId);
        var requestUri = $"/api/v1/{encodedpartyId}/purchaseOrder/send";

        if (!string.IsNullOrEmpty(receiverPartyId))
            requestUri += "?receiverId=" + HttpUtility.UrlEncode(receiverPartyId);

        var res = await _psbClient.PostFile<Document>(requestUri, file, cancellation);
        return res;
    }
}