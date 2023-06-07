using System.Collections.Generic;
using EConnect.Psb.Models;
using System.Threading;
using System.Threading.Tasks;
using EConnect.Psb.Models.Peppol;

namespace EConnect.Psb.Api;

public interface IPsbPeppolApi : IPsbPeppolLookupApi
{
    Task<PeppolConfig> GetEnvironmentConfig(CancellationToken cancellation = default);

    Task<PeppolConfig> PutEnvironmentConfig(PeppolConfig config, CancellationToken cancellation = default);

    Task<Party[]> GetParties(CancellationToken cancellation = default);

    Task<PeppolConfig> GetConfig(string partyId, CancellationToken cancellation = default);

    Task<PeppolConfig> PutConfig(string partyId, PeppolConfig config, CancellationToken cancellation = default);

    Task DeleteConfig(string partyId, CancellationToken cancellation = default);
}