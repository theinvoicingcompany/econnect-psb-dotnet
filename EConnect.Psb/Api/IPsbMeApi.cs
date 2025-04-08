using System.Threading;
using System.Threading.Tasks;
using EConnect.Psb.Models;

namespace EConnect.Psb.Api;

public interface IPsbMeApi
{
    Task<Me> Me(string? domainId = null, CancellationToken cancellation = default);

    Task<UserParty[]> MeParties(string? domainId = null, CancellationToken cancellation = default);
}