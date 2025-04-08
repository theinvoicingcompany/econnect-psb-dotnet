using EConnect.Psb.Models;
using System.Threading;
using System.Threading.Tasks;
using EConnect.Psb.Models.Peppol;

namespace EConnect.Psb.Api;

public interface IPsbPeppolApi : IPsbPeppolLookupApi
{
    Task<PeppolConfig> GetEnvironmentConfig(string? domainId = null, CancellationToken cancellation = default);

    Task<PeppolConfig> PutEnvironmentConfig(PeppolConfig config, string? domainId = null, CancellationToken cancellation = default);

    Task<ListResult<Party>> GetParties(string? domainId = null, CancellationToken cancellation = default);

    Task<PeppolConfig> GetConfig(string partyId, string? domainId = null, CancellationToken cancellation = default);

    Task<PeppolConfig> PutConfig(string partyId, PeppolConfig config, string? domainId = null, CancellationToken cancellation = default);

    Task DeleteConfig(string partyId, string? domainId = null, CancellationToken cancellation = default);
}