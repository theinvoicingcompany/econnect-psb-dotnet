using EConnect.Psb.Models;
using System.Threading;
using System.Threading.Tasks;

namespace EConnect.Psb.Api;

public interface IPsbPurchaseOrderApi
{
    Task<Party> QueryRecipientParty(string partyId, string[] recipientPartyIds, string? preferredDocumentTypeId = null, CancellationToken cancellation = default);

    Task<Document> Send(string partyId, FileContent file, string? receiverId = null, CancellationToken cancellation = default);

}