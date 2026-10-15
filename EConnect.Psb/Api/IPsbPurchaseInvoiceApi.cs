using System.Collections.Generic;
using EConnect.Psb.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EConnect.Psb.Api;

public interface IPsbPurchaseInvoiceApi
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

    Task<Document> Response(
        string partyId,
        string documentId,
        InvoiceResponse purchaseInvoice,
        string? responseDocumentId = null,
        string? domainId = null,
        CancellationToken cancellation = default);

    Task Delete(
        string partyId,
        string documentId,
        string? domainId = null,
        CancellationToken cancellation = default);

    Task<Document> Recognize(
        string partyId,
        FileContent file,
        string? channel = null,
        string? documentId = null,
        string? domainId = null,
        CancellationToken cancellation = default);
}