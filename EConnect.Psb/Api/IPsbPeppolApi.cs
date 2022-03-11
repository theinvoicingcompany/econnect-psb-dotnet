using EConnect.Psb.Models;
using System.Threading;
using System.Threading.Tasks;
using EConnect.Psb.Models.Peppol;
using System.Collections.Generic;

namespace EConnect.Psb.Api;

public interface IPsbPeppolApi
{
    Task<DeliveryOption[]> GetDeliveryOption(CancellationToken cancellation = default);

    Task<PeppolPartyConfig> GetEnviromentConfig(CancellationToken cancellation = default);

    Task<PeppolPartyConfig> PutEnviromentConfig(PeppolPartyConfig config, CancellationToken cancellation = default);

    Task<Party[]> GetParties(CancellationToken cancellation = default);

    Task<PeppolPartyConfig> GetPartyConfig(string partyId, CancellationToken cancellation = default);

    Task<PeppolPartyConfig> PutPartyConfig(string partyId, PeppolPartyConfig partyConfig, CancellationToken cancellation = default);

    Task DeletePartyConfig(string partyId, CancellationToken cancellation = default);
}