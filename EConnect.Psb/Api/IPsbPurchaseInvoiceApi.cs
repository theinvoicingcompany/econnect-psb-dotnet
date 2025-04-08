using EConnect.Psb.Models;
using System.Threading;
using System.Threading.Tasks;

namespace EConnect.Psb.Api;

public interface IPsbPurchaseInvoiceApi
{
    Task<FileContent> Download(
        string partyId,
        string documentId,
        string? domainId = null,
        CancellationToken cancellation = default);

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