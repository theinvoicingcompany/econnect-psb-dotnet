using EConnect.Psb.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EConnect.Psb.Api;

public interface IPsbPurchaseInvoiceApi
{
    Task<string> Download(string partyId, string documentId, CancellationToken cancellation = default);

    Task<Document> Response(string partyId, string documentId, InvoiceResponse purchaseInvoice, CancellationToken cancellation = default);
    
    Task Delete(string partyId, string documentId, CancellationToken cancellation = default);
    
    Task<Document> Recognize(string partyId, string file, CancellationToken cancellation = default);
}