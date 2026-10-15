using System.Collections.Generic;
using EConnect.Psb.Models;
using System;
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
        string? documentFamily = null,
        CancellationToken cancellation = default);

    // Pre-documentFamily signature, kept for binary compatibility. No defaults, so it only binds when
    // all five arguments are passed and never makes a shorter call ambiguous.
    [Obsolete("Use the QueryRecipientParty overload that takes documentFamily.")]
    Task<Party> QueryRecipientParty(
        string senderPartyId,
        IEnumerable<string> recipientPartyIds,
        string? preferredDocumentTypeId,
        string? domainId,
        CancellationToken cancellation);

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