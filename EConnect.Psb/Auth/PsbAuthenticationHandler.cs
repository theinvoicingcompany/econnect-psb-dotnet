using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace EConnect.Psb.Auth;

public class PsbAuthenticationHandler : DelegatingHandler
{
    private readonly IPsbAuthenticationProvider _authentication;

    public PsbAuthenticationHandler(IPsbAuthenticationProvider authentication)
    {
        _authentication = authentication;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await _authentication.GetAccessToken("openid ap", cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return await base.SendAsync(request, cancellationToken);
    }
}