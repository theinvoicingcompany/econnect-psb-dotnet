using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EConnect.Psb.Client.Extensions;
using EConnect.Psb.Config;
using EConnect.Psb.Models;
using Microsoft.Extensions.Options;

[assembly: InternalsVisibleTo("EConnect.Psb.UnitTests")]
namespace EConnect.Psb.Client;

public class PsbClient
{
    private readonly HttpClient _httpClient;

    public PsbClient(HttpClient httpClient, IOptions<PsbOptions> options)
    {
        _httpClient = httpClient;

        httpClient.BaseAddress = new Uri($"{options.Value.PsbUrl}");
        httpClient.DefaultRequestHeaders.Add("Subscription-Key", options.Value.SubscriptionKey);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    private Task<HttpResponseMessage> Send(HttpRequestMessage request, string? domainId, CancellationToken cancellation)
    {
        if (!string.IsNullOrEmpty(domainId))
            request.Headers.Add("X-EConnect-DomainId", domainId);

        return _httpClient.SendAsync(request, cancellation);
    }

    public async Task<TResponseBody> Get<TResponseBody>(
        string? requestUri,
        string? domainId = null,
        CancellationToken cancellation = default
        ) where TResponseBody : class
    {
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

        var res = await Send(request, domainId, cancellation).ConfigureAwait(false);

        if (typeof(TResponseBody) == typeof(FileContent) && new FileContent(res.Content, "file") is TResponseBody body)
        {
            return body;
        }

        return await res.Read<TResponseBody>(cancellation).ConfigureAwait(false);
    }

    public async Task<TResponseBody> Put<TResponseBody>(
        string? requestUri,
        object body,
        string? domainId = null,
        CancellationToken cancellation = default) where TResponseBody : class
    {
        var request = new HttpRequestMessage(HttpMethod.Put, requestUri)
        {
            Content = JsonContent.Create(body)
        };

        var res = await Send(request, domainId, cancellation).ConfigureAwait(false);
        return await res.Read<TResponseBody>(cancellation).ConfigureAwait(false);
    }

    public async Task<TResponseBody> Post<TResponseBody>(
        string? requestUri,
        object body,
        string? documentId = null,
        string? domainId = null,
        CancellationToken cancellation = default
        ) where TResponseBody : class
    {
        var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = JsonContent.Create(body)
        };

        if (!string.IsNullOrEmpty(documentId))
            request.Headers.Add("X-EConnect-DocumentId", documentId);

        var res = await Send(request, domainId, cancellation).ConfigureAwait(false);
        return await res.Read<TResponseBody>(cancellation).ConfigureAwait(false);
    }

    public async Task<TResponseBody> PostFile<TResponseBody>(
        string? requestUri,
        FileContent file,
        string? documentId = null,
        string? domainId = null,
        IDictionary<string, string>? metaAttributes = null,
        CancellationToken cancellation = default
        ) where TResponseBody : class
    {
        using var multipart = new MultipartFormDataContent();
        if (string.IsNullOrEmpty(file.Filename))
            multipart.Add(file.Content, "file");
        else
            multipart.Add(file.Content, "file", file.Filename);

        if (metaAttributes?.Any() == true)
        {
            var json = JsonSerializer.Serialize(metaAttributes);
            multipart.Add(new StringContent(json), "metaAttributes");
        }

        var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = multipart
        };

        if (!string.IsNullOrEmpty(documentId))
            request.Headers.Add("X-EConnect-DocumentId", documentId);

        var res = await Send(request, domainId, cancellation).ConfigureAwait(false);
        file.Dispose();

        return await res.Read<TResponseBody>(cancellation).ConfigureAwait(false);
    }

    public async Task Delete(
        string? requestUri,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);

        var res = await Send(request, domainId, cancellation).ConfigureAwait(false);

        if (!res.IsSuccessStatusCode)
            await res.ThrowError(cancellation).ConfigureAwait(false);
    }
}
