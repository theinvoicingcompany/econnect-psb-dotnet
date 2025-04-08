using System.Threading;
using System.Threading.Tasks;
using EConnect.Psb.Models;

namespace EConnect.Psb.Api;

public interface IPsbSalesOrderApi
{
    Task<FileContent> Download(
        string partyId,
        string documentId,
        string? domainId = null,
        CancellationToken cancellation = default);

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