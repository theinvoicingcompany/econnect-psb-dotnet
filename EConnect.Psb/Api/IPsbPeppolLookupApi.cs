using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EConnect.Psb.Models;

namespace EConnect.Psb.Api;

public interface IPsbPeppolLookupApi
{
    Task<DeliveryOption[]> GetDeliveryOptions(
        List<string> partyIds,
        string? preferredDocumentTypeId = null,
        List<string>? documentTypeIds = null,
        string? documentFamily = null,
        bool? isCredit = null,
        CancellationToken cancellation = default);
}