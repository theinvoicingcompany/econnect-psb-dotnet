using System.Threading;
using System.Threading.Tasks;
using EConnect.Psb.Models;

namespace EConnect.Psb.Api;

public interface IPsbEnrollmentApi
{
    Task<Document> Enroll(
        string partyId,
        Enrollment enrollment,
        string? domainId = null,
        CancellationToken cancellation = default);

    Task<Document> Unenroll(
        string partyId,
        string? domainId = null,
        CancellationToken cancellation = default);
}
