using System;
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

    private async Task<Document> PostDocument(
        string requestUri,
        FileContent file,
        string counterpartyKey,
        string? counterpartyId,
        GenericDocumentOptions? options,
        string? domainId,
        CancellationToken cancellation)
    {
        var query = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(counterpartyId))
            query.Add(counterpartyKey, counterpartyId!);

        if (!string.IsNullOrEmpty(options?.Topic))
            query.Add("topic", options!.Topic!);

        if (!string.IsNullOrEmpty(options?.Channel))
            query.Add("channel", options!.Channel!);

        if (options?.DisablePlugins == true)
            query.Add("disablePlugins", bool.TrueString);

        if (!string.IsNullOrEmpty(options?.SourceFormat))
            query.Add("sourceFormat", options!.SourceFormat!);

        if (!string.IsNullOrEmpty(options?.TargetFormat))
            query.Add("targetFormat", options!.TargetFormat!);

        if (!string.IsNullOrEmpty(options?.Flow))
            query.Add("flow", options!.Flow!);

        var queryString = await new FormUrlEncodedContent(query).ReadAsStringAsync().ConfigureAwait(false);
        if (!string.IsNullOrEmpty(queryString))
            requestUri += "?" + queryString;

        var res = await _psbClient.PostFile<Document>(
            requestUri,
            file,
            options?.DocumentId,
            domainId,
            options?.MetaAttributes,
            cancellation: cancellation).ConfigureAwait(false);

        return res;
    }

    public Task<Document> Receive(
        string receiverPartyId,
        FileContent file,
        GenericReceiveOptions? options = null,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var requestUri = $"/api/v1/{HttpUtility.UrlEncode(receiverPartyId)}/generic/receive";

        return PostDocument(requestUri, file, "senderId", options?.SenderPartyId, options, domainId, cancellation);
    }

    [Obsolete("Use the Receive overload that takes GenericReceiveOptions.")]
    public Task<Document> Receive(
        string receiverPartyId,
        FileContent file,
        string? topic,
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
        var options = new GenericReceiveOptions
        {
            Topic = topic,
            SenderPartyId = senderPartyId,
            Channel = channel,
            DisablePlugins = disablePlugins,
            DocumentId = documentId,
            SourceFormat = sourceFormat,
            TargetFormat = targetFormat,
            MetaAttributes = metaAttributes
        };

        return Receive(receiverPartyId, file, options, domainId, cancellation);
    }

    public Task<Document> Send(
        string senderPartyId,
        FileContent file,
        GenericSendOptions? options = null,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var requestUri = $"/api/v1/{HttpUtility.UrlEncode(senderPartyId)}/generic/send";

        return PostDocument(requestUri, file, "receiverId", options?.ReceiverPartyId, options, domainId, cancellation);
    }

    [Obsolete("Use the Send overload that takes GenericSendOptions.")]
    public Task<Document> Send(
        string senderPartyId,
        FileContent file,
        string? topic,
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
        var options = new GenericSendOptions
        {
            Topic = topic,
            ReceiverPartyId = receiverPartyId,
            Channel = channel,
            DisablePlugins = disablePlugins,
            DocumentId = documentId,
            SourceFormat = sourceFormat,
            TargetFormat = targetFormat,
            MetaAttributes = metaAttributes
        };

        return Send(senderPartyId, file, options, domainId, cancellation);
    }

    public async Task<FileContent> Download(
        string partyId,
        string documentId,
        string? targetFormat = null,
        string? domainId = null,
        bool disablePlugins = false,
        bool ignoreValidationErrors = false,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var encodedDocumentId = HttpUtility.UrlEncode(documentId);
        var requestUri = $"/api/v1/{encodedPartyId}/generic/{encodedDocumentId}/download";
        var query = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(targetFormat))
            query.Add("targetFormat", targetFormat!);

        if (disablePlugins)
            query.Add("disablePlugins", disablePlugins.ToString());

        if (ignoreValidationErrors)
            query.Add("ignoreValidationErrors", ignoreValidationErrors.ToString());

        var queryString = await new FormUrlEncodedContent(query).ReadAsStringAsync().ConfigureAwait(false);
        if (!string.IsNullOrEmpty(queryString))
            requestUri += "?" + queryString;

        var file = await _psbClient.Get<FileContent>(
            requestUri,
            domainId,
            cancellation).ConfigureAwait(false);

        return file;
    }

    [Obsolete("Use the Download overload that takes disablePlugins and ignoreValidationErrors.")]
    public Task<FileContent> Download(
        string partyId,
        string documentId,
        string? targetFormat,
        string? domainId,
        CancellationToken cancellation)
        => Download(partyId, documentId, targetFormat, domainId, false, false, cancellation);

    public async Task<FileContent> Transform(
        FileContent file,
        string targetFormat,
        string? sourceFormat = null,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var requestUri = "/api/v1/generic/transform";
        var query = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(sourceFormat))
            query.Add("sourceFormat", sourceFormat!);

        query.Add("targetFormat", targetFormat);

        var queryString = await new FormUrlEncodedContent(query).ReadAsStringAsync().ConfigureAwait(false);
        requestUri += "?" + queryString;

        var res = await _psbClient.PostFile<FileContent>(
            requestUri,
            file,
            domainId: domainId,
            cancellation: cancellation).ConfigureAwait(false);

        return res;
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
