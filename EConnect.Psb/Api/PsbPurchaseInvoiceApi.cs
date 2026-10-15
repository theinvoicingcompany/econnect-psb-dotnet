using System.Collections.Generic;
using System;
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

    public async Task<Party> QueryRecipientParty(
        string senderPartyId,
        IEnumerable<string> recipientPartyIds,
        string? preferredDocumentTypeId = null,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(senderPartyId);
        var requestUri = $"/api/v1/{encodedPartyId}/purchaseInvoice/queryRecipientParty";

        if (!string.IsNullOrEmpty(preferredDocumentTypeId))
            requestUri += "?preferredDocumentTypeId=" + HttpUtility.UrlEncode(preferredDocumentTypeId);

        var party = await _psbClient.Post<Party>(requestUri, recipientPartyIds, domainId: domainId, cancellation: cancellation).ConfigureAwait(false);

        return party;
    }

    public async Task<FileContent> Download(
        string partyId,
        string documentId,
        string? domainId = null,
        string? targetFormat = null,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var encodedDocumentId = HttpUtility.UrlEncode(documentId);
        var targetUrl = $"/api/v1/{encodedPartyId}/purchaseInvoice/{encodedDocumentId}/download";

        if (!string.IsNullOrEmpty(targetFormat))
            targetUrl += "?targetFormat=" + HttpUtility.UrlEncode(targetFormat);

        var file = await _psbClient.Get<FileContent>(targetUrl, domainId, cancellation).ConfigureAwait(false);
        return file;
    }

    [Obsolete("Use the Download overload that takes targetFormat.")]
    public Task<FileContent> Download(
        string partyId,
        string documentId,
        string? domainId,
        CancellationToken cancellation)
        => Download(partyId, documentId, domainId, null, cancellation);

    public async Task<Document> Response(
        string partyId,
        string documentId,
        InvoiceResponse purchaseInvoice,
        string? responseDocumentId = null,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var encodedDocumentId = HttpUtility.UrlEncode(documentId);
        var targetUrl = $"/api/v1/{encodedPartyId}/purchaseInvoice/{encodedDocumentId}/response";

        var invoiceResponse = await _psbClient.Post<Document>(
            targetUrl,
            purchaseInvoice,
            responseDocumentId,
            domainId,
            cancellation
        ).ConfigureAwait(false);

        return invoiceResponse;
    }

    public async Task Delete(
        string partyId,
        string documentId,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var encodedDocumentId = HttpUtility.UrlEncode(documentId);
        var targetUrl = $"/api/v1/{encodedPartyId}/purchaseInvoice/{encodedDocumentId}";

        await _psbClient.Delete(
            targetUrl,
            domainId,
            cancellation
        ).ConfigureAwait(false);
    }

    public async Task<Document> Recognize(
        string partyId,
        FileContent file,
        string? channel = null,
        string? documentId = null,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var requestUri = $"/api/v1/{encodedPartyId}/purchaseInvoice/recognize";

        if (!string.IsNullOrEmpty(channel))
            requestUri += "?channel=" + HttpUtility.UrlEncode(channel);

        var document = await _psbClient.PostFile<Document>(
                requestUri,
                file,
                documentId,
                domainId,
                cancellation: cancellation)
            .ConfigureAwait(false);

        return document;
    }
}