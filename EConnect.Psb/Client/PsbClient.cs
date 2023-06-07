﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
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

    public async Task<TResponseBody> Get<TResponseBody>(
        string? requestUri,
        CancellationToken cancellation = default
        ) where TResponseBody : class
    {
        var res = await _httpClient.GetAsync(requestUri, cancellation).ConfigureAwait(false);

        if (typeof(TResponseBody) == typeof(FileContent) && new FileContent(res.Content, "file") is TResponseBody body)
        {
            return body;
        }

        return await res.Read<TResponseBody>(cancellation).ConfigureAwait(false);
    }

    public async Task<TResponseBody> Put<TResponseBody>(
        string? requestUri,
        object body,
        CancellationToken cancellation = default
        ) where TResponseBody : class
    {
        var res = await _httpClient.PutAsJsonAsync(requestUri, body, cancellation).ConfigureAwait(false);
        return await res.Read<TResponseBody>(cancellation).ConfigureAwait(false);
    }

    public async Task<TResponseBody> Post<TResponseBody>(
        string? requestUri,
        object body,
        string? documentId = null,
        CancellationToken cancellation = default
        ) where TResponseBody : class
    {
        var content = JsonContent.Create(body);
       
        if(!string.IsNullOrEmpty(documentId))
            content.Headers.Add("X-EConnect-DocumentId", documentId);

        var res = await _httpClient.PostAsync(requestUri, content, cancellation).ConfigureAwait(false);
        return await res.Read<TResponseBody>(cancellation).ConfigureAwait(false);
    }

    public async Task<TResponseBody> PostFile<TResponseBody>(
        string? requestUri,
        FileContent file,
        string? documentId = null,
        CancellationToken cancellation = default
        ) where TResponseBody : class
    {
        using var multipart = new MultipartFormDataContent();
        if (string.IsNullOrEmpty(file.Filename))
            multipart.Add(file.Content, "file");
        else
            multipart.Add(file.Content, "file", file.Filename);

        if(!string.IsNullOrEmpty(documentId))
            multipart.Headers.Add("X-EConnect-DocumentId", documentId);

        var res = await _httpClient.PostAsync(requestUri, multipart, cancellation).ConfigureAwait(false);
        file.Dispose();

        return await res.Read<TResponseBody>(cancellation).ConfigureAwait(false);
    }

    public async Task Delete(
        string? requestUri,
        CancellationToken cancellation = default)
    {
        var res = await _httpClient.DeleteAsync(requestUri, cancellation).ConfigureAwait(false);

        if (!res.IsSuccessStatusCode)
            await res.ThrowError(cancellation).ConfigureAwait(false);
    }
}
