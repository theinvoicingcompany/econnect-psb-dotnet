﻿using System.Threading;
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
        CancellationToken cancellation)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var encodedDocumentId = HttpUtility.UrlEncode(documentId);

        var targetUrl = $"/api/v1/{encodedPartyId}/salesOrder/{encodedDocumentId}/download";

        var file = await _psbClient.Get<FileContent>(
            requestUri: targetUrl,
            cancellation: cancellation).ConfigureAwait(false);

        return file;
    }

    public async Task Delete(
        string partyId,
        string documentId,
        CancellationToken cancellation)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var encodedDocumentId = HttpUtility.UrlEncode(documentId);
        var targetUrl = $"/api/v1/{encodedPartyId}/salesOrder/{encodedDocumentId}";

        await _psbClient.Delete(
            requestUri: targetUrl,
            cancellation: cancellation).ConfigureAwait(false);
    }

    public async Task<Document> Response(
        string partyId, 
        string documentId,
        OrderResponse orderResponse,
        string? responseDocumentId = null,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var encodedDocumentId = HttpUtility.UrlEncode(documentId);
        var targetUrl = $"/api/v1/{encodedPartyId}/salesOrder/{encodedDocumentId}/response";

        var res = await _psbClient.Post<Document>(
            targetUrl,
            orderResponse,
            responseDocumentId,
            cancellation).ConfigureAwait(false);

        return res;
    }
}