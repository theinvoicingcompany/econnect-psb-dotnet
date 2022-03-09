using System.Threading;
using System.Threading.Tasks;
using EConnect.Psb.Models;

namespace EConnect.Psb.Api;

public interface IPsbGenericApi
{
    Task<Document> Receive(string receiverPartyId,
        FileContent file, string? topic = null, string? senderPartyId = null,
        CancellationToken cancellation = default);
}