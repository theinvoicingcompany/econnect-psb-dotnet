using EConnect.Psb.Models;
using System.Threading;
using System.Threading.Tasks;

namespace EConnect.Psb.Api;

public interface IPsbPurchaseInvoiceApi
{
    Task<FileContent> Download(string partyId, string documentId, CancellationToken cancellation = default);

    Task<Document> Response(string partyId, string documentId, InvoiceResponse purchaseInvoice, CancellationToken cancellation = default);
    
    Task Delete(string partyId, string documentId, CancellationToken cancellation = default);
    
    Task<Document> Recognize(string partyId, FileContent file, CancellationToken cancellation = default);
}