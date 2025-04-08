using System.Threading;
using System.Threading.Tasks;
using EConnect.Psb.Models;

namespace EConnect.Psb.Api;

public interface IPsbUserApi
{
    Task<User[]> GetUsers(string? domainId = null, CancellationToken cancellation = default);

    Task<User> SetUser(User user, string? domainId = null, CancellationToken cancellation = default);

    Task DeleteUser(string name, string? domainId = null, CancellationToken cancellation = default);

    Task<UserParty[]> GetUserParties(string name, string? domainId = null, CancellationToken cancellation = default);

    Task<UserParty> SetUserParty(string name, UserParty userParties, string? domainId = null, CancellationToken cancellation = default);

    Task DeleteUserParty(string name, string partyId, string? domainId = null, CancellationToken cancellation = default);
}