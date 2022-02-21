using System.Threading;
using System.Threading.Tasks;

namespace EConnect.Psb.Auth;

public interface IPsbAuthenticationProvider
{
    Task<string> GetAccessToken(string scope, CancellationToken cancellation = default);
}