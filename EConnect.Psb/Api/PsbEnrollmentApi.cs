using System.Threading;
using System.Threading.Tasks;
using System.Web;
using EConnect.Psb.Client;
using EConnect.Psb.Models;

namespace EConnect.Psb.Api;

public class PsbEnrollmentApi : IPsbEnrollmentApi
{
    private readonly PsbClient _psbClient;

    public PsbEnrollmentApi(PsbClient psbClient)
    {
        _psbClient = psbClient;
    }

    public Task<Document> Enroll(
        string partyId,
        Enrollment enrollment,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var targetUrl = $"/api/v1/{encodedPartyId}/enroll";

        return _psbClient.Put<Document>(targetUrl, enrollment, domainId, cancellation);
    }

    public Task<Document> Unenroll(
        string partyId,
        string? domainId = null,
        CancellationToken cancellation = default)
    {
        var encodedPartyId = HttpUtility.UrlEncode(partyId);
        var targetUrl = $"/api/v1/{encodedPartyId}/enroll";

        return _psbClient.Delete<Document>(targetUrl, domainId, cancellation);
    }
}
