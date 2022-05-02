using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using EConnect.Psb.Client;
using EConnect.Psb.Models;

namespace EConnect.Psb.Api;

public class PsbGenericApi : IPsbGenericApi
{
    private readonly PsbClient _psbClient;
    public PsbGenericApi(PsbClient psbClient)
    {
        _psbClient = psbClient;
    }

    public async Task<Document> Receive(string receiverPartyId,
        FileContent file, string? topic = null, string? senderPartyId = null,
        CancellationToken cancellation = default)
    {
        var requestUri = $"/api/v1-beta/{HttpUtility.UrlEncode(receiverPartyId)}/generic/receive";
        var query = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(senderPartyId))
            query.Add("senderId", senderPartyId!);

        if (!string.IsNullOrEmpty(topic))
            query.Add("topic", topic!);

        var queryString = await new FormUrlEncodedContent(query).ReadAsStringAsync().ConfigureAwait(false);
        if (!string.IsNullOrEmpty(queryString))
            requestUri += "?" + queryString;

        var res = await _psbClient.PostFile<Document>(requestUri, file, cancellation).ConfigureAwait(false);
        return res;
    }

    public async Task<Document> Send(
        string senderPartyId,
        FileContent file,
        string? topic = null,
        string? receiverPartyId = null,
        CancellationToken cancellation = default)
    {
        var encodedSenderPartyId = HttpUtility.UrlEncode(senderPartyId);
        var requestUri = $"/api/v1-beta/{encodedSenderPartyId}/generic/send";
        var query = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(receiverPartyId))
            query.Add("receiverId", receiverPartyId!);

        if (!string.IsNullOrEmpty(topic))
            query.Add("topic", topic!);

        var queryString = await new FormUrlEncodedContent(query).ReadAsStringAsync().ConfigureAwait(false);
        if (!string.IsNullOrEmpty(queryString))
            requestUri += "?" + queryString;

        var res = await _psbClient.PostFile<Document>(
            requestUri,
            file,
            cancellation).ConfigureAwait(false);

        return res;
    }

    public async Task<FileContent> Download(
        string partyId,
        string documentId,
        string? targetFormat = null,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var encodedDocumentId = HttpUtility.UrlEncode(documentId);
        var requestUri = $"/api/v1-beta/{encodedPartyId}/generic/{encodedDocumentId}/download";
        var query = new Dictionary<string, string>();


        if (!string.IsNullOrEmpty(targetFormat))
            query.Add("targetFormat", targetFormat!);

        var queryString = await new FormUrlEncodedContent(query).ReadAsStringAsync().ConfigureAwait(false);
        if (!string.IsNullOrEmpty(queryString))
            requestUri += "?" + queryString;

        var file = await _psbClient.Get<FileContent>(
            requestUri,
            cancellation).ConfigureAwait(false);

        return file;
    }
}