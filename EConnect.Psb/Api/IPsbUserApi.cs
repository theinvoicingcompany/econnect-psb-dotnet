using System.Threading;
using System.Threading.Tasks;
using EConnect.Psb.Models;

namespace EConnect.Psb.Api;

public interface IPsbUserApi
{
    Task<User[]> GetUsers(CancellationToken cancellation = default);

    Task<User> SetUser(User user, CancellationToken cancellation = default);

    Task DeleteUser(string name, CancellationToken cancellation = default);

    Task<UserParty[]> GetUserParties(string name, CancellationToken cancellation = default);

    Task<UserParty> SetUserParty(string name, UserParty userParties, CancellationToken cancellation = default);

    Task DeleteUserParty(string name, string partyId, CancellationToken cancellation = default);
}