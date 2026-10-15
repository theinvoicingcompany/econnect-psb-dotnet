using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using EConnect.Psb.Models;

namespace EConnect.Psb.Api;

public interface IPsbSalesOrderApi
{
    Task<Party> QueryRecipientParty(
        string senderPartyId,
        IEnumerable<string> recipientPartyIds,
        string? preferredDocumentTypeId = null,
        string? domainId = null,
        CancellationToken cancellation = default);

    Task<FileContent> Download(
        string partyId,
        string documentId,
        string? domainId = null,
        string? targetFormat = null,
        CancellationToken cancellation = default);

    // Pre-targetFormat signature, kept for binary compatibility. No defaults, so it only binds when
    // all four arguments are passed and never makes a shorter call ambiguous.
    [Obsolete("Use the Download overload that takes targetFormat.")]
    Task<FileContent> Download(
        string partyId,
        string documentId,
        string? domainId,
        CancellationToken cancellation);

    Task Delete(
        string partyId,
        string documentId,
        string? domainId = null,
        CancellationToken cancellation = default);

    Task<Document> Response(
        string partyId,
        string documentId,
        OrderResponse orderResponse,
        string? responseDocumentId = null,
        string? domainId = null,
        CancellationToken cancellation = default);
}
