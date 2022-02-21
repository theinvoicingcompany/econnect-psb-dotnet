using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EConnect.Psb.Models;

namespace EConnect.Psb.Api;

public interface IPsbMeApi
{
    Task<Me> Me(CancellationToken cancellation = default);

    Task<IEnumerable<UserParty>> MeParties(CancellationToken cancellation = default);
}