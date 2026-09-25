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

    public async Task<Document> Receive(
        string receiverPartyId,
        FileContent file,
        string? topic = null,
        string? senderPartyId = null,
        string? channel = null,
        bool disablePlugins = false,
        string? documentId = null,
        string? sourceFormat = null,
        string? targetFormat = null,
        IDictionary<string, string>? metaAttributes = null,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var requestUri = $"/api/v1-beta/{HttpUtility.UrlEncode(receiverPartyId)}/generic/receive";
        var query = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(senderPartyId))
            query.Add("senderId", senderPartyId!);

        if (!string.IsNullOrEmpty(topic))
            query.Add("topic", topic!);

        if (!string.IsNullOrEmpty(channel))
            query.Add("channel", channel!);

        if (disablePlugins)
            query.Add("disablePlugins", disablePlugins.ToString());

        if (!string.IsNullOrEmpty(sourceFormat))
            query.Add("sourceFormat", sourceFormat!);

        if (!string.IsNullOrEmpty(targetFormat))
            query.Add("targetFormat", targetFormat!);

        var queryString = await new FormUrlEncodedContent(query).ReadAsStringAsync().ConfigureAwait(false);
        if (!string.IsNullOrEmpty(queryString))
            requestUri += "?" + queryString;

        var res = await _psbClient.PostFile<Document>(
            requestUri,
            file,
            documentId,
            domainId,
            metaAttributes,
            cancellation: cancellation).ConfigureAwait(false);

        return res;
    }

    public async Task<Document> Send(
        string senderPartyId,
        FileContent file,
        string? topic = null,
        string? receiverPartyId = null,
        string? channel = null,
        bool disablePlugins = false,
        string? documentId = null,
        string? sourceFormat = null,
        string? targetFormat = null,
        IDictionary<string, string>? metaAttributes = null,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var encodedSenderPartyId = HttpUtility.UrlEncode(senderPartyId);
        var requestUri = $"/api/v1-beta/{encodedSenderPartyId}/generic/send";
        var query = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(receiverPartyId))
            query.Add("receiverId", receiverPartyId!);

        if (!string.IsNullOrEmpty(topic))
            query.Add("topic", topic!);

        if (!string.IsNullOrEmpty(channel))
            query.Add("channel", channel!);

        if (disablePlugins)
            query.Add("disablePlugins", disablePlugins.ToString());

        if (!string.IsNullOrEmpty(sourceFormat))
            query.Add("sourceFormat", sourceFormat!);

        if (!string.IsNullOrEmpty(targetFormat))
            query.Add("targetFormat", targetFormat!);

        var queryString = await new FormUrlEncodedContent(query).ReadAsStringAsync().ConfigureAwait(false);
        if (!string.IsNullOrEmpty(queryString))
            requestUri += "?" + queryString;

        var res = await _psbClient.PostFile<Document>(
            requestUri,
            file,
            documentId,
            domainId,
            metaAttributes,
            cancellation: cancellation).ConfigureAwait(false);

        return res;
    }

    public async Task<FileContent> Download(
        string partyId,
        string documentId,
        string? targetFormat = null,
        string? domainId = null,
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
            domainId,
            cancellation).ConfigureAwait(false);

        return file;
    }

    public Task<ValidationOption[]> GetValidationOptions(
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        return _psbClient.Get<ValidationOption[]>(
            "/api/v1/generic/validate/options",
            domainId,
            cancellation);
    }

    public async Task<DocumentValidationResult> Validate(
        FileContent file,
        string? sourceFormat = null,
        string? optionId = null,
        string? languageCode = null,
        IDictionary<string, string>? metaAttributes = null,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var requestUri = "/api/v1/generic/validate";
        var query = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(sourceFormat))
            query.Add("sourceFormat", sourceFormat!);

        if (!string.IsNullOrEmpty(optionId))
            query.Add("optionId", optionId!);

        var queryString = await new FormUrlEncodedContent(query).ReadAsStringAsync().ConfigureAwait(false);
        if (!string.IsNullOrEmpty(queryString))
            requestUri += "?" + queryString;

        var headers = new Dictionary<string, string>();
        if (!string.IsNullOrEmpty(languageCode))
            headers.Add("X-EConnect-Language", languageCode!);

        var res = await _psbClient.PostFile<DocumentValidationResult>(
            requestUri,
            file,
            null,
            domainId,
            metaAttributes,
            headers,
            cancellation).ConfigureAwait(false);

        return res;
    }
}