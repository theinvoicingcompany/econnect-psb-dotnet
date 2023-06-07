using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EConnect.Psb.Config;
using EConnect.Psb.Models;
using IdentityModel.Client;
using Microsoft.Extensions.Options;

namespace EConnect.Psb.Auth;

public class PsbAuthenticationProvider : IPsbAuthenticationProvider
{
    private readonly PsbOptions _options;
    private readonly IHttpClientFactory _httpClientFactory;

    public PsbAuthenticationProvider(IOptions<PsbOptions> options, IHttpClientFactory httpClientFactory)
    {
        _options = options.Value;
        _httpClientFactory = httpClientFactory;
    }

    private async Task<TokenResponse> RequestAccessToken(string scope, CancellationToken cancellation)
    {
        using var request = new PasswordTokenRequest
        {
            Address = _options.IdentityUrl + "/connect/token",
            ClientId = _options.ClientId,
            ClientSecret = _options.ClientSecret,
            Scope = scope,
            UserName = _options.Username,
            Password = _options.Password
        };

        var httpClient = _httpClientFactory.CreateClient(nameof(PsbAuthenticationProvider));
        var response = await httpClient.RequestPasswordTokenAsync(request, cancellation).ConfigureAwait(false);

        if (response.IsError)
        {
            throw new EConnectException("OAUTH01", response.Error);
        }

        if (response.AccessToken == null)
        {
            throw new EConnectException("OAUTH02", "Something went wrong authenticating. Could not get access token.");
        }

        return response;
    }

    private readonly Dictionary<string, (DateTimeOffset TokenExpiresAt, string AccessToken)> _cache = new();

    public async Task<string> GetAccessToken(string scope, CancellationToken cancellation = default)
    {
        if (_cache.TryGetValue(scope, out var cacheEntry) &&
            cacheEntry.TokenExpiresAt >= DateTimeOffset.Now + TimeSpan.FromMinutes(1))
            return cacheEntry.AccessToken;

        var tokenResponse = await RequestAccessToken(scope, cancellation).ConfigureAwait(false);

        _cache[scope] = (DateTimeOffset.Now.AddSeconds(tokenResponse.ExpiresIn),
                 tokenResponse.AccessToken!);
        
        return tokenResponse.AccessToken!;
    }
}