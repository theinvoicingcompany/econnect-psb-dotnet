using System.Collections.Generic;
using EConnect.Psb.Models;
using System.Threading;
using System.Threading.Tasks;

namespace EConnect.Psb.Api;

public interface IPsbPurchaseOrderApi
{
    Task<Party> QueryRecipientParty(
        string senderPartyId,
        IEnumerable<string> recipientPartyIds,
        string? preferredDocumentTypeId = null,
        string? domainId = null,
        CancellationToken cancellation = default);

    Task<Document> Send(string partyId,
        FileContent file,
        string? receiverId = null,
        string? channel = null,
        string? documentId = null,
        string? domainId = null,
        CancellationToken cancellation = default);

    Task<Document> Cancel(
        string partyId,
        string documentId,
        OrderCancellation message,
        string? cancelDocumentId = null,
        string? domainId = null,
        CancellationToken cancellation = default);
}