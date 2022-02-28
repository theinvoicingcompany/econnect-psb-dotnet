using System.Threading;
using System.Threading.Tasks;
using EConnect.Psb.Models;

namespace EConnect.Psb.Api;

public interface IPsbSalesInvoiceApi
{
    Task<string> QueryRecipientParty(string senderPartyId, string[] recipientPartyIds, string? preferredDocumentTypeId = null, CancellationToken cancellation = default);

    Task<string> Send(string senderPartyId, FileContent file, string? receiverPartyId = null, CancellationToken cancellation = default);

    Task<string> Recognize(string senderPartyId, FileContent file, CancellationToken cancellation = default);
}