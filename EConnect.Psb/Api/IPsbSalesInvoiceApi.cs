using System.Threading;
using System.Threading.Tasks;

namespace EConnect.Psb.Api;

public interface IPsbSalesInvoiceApi
{
    Task<string> QueryRecipientParty(string senderPartyId, string[] recipientPartyIds, CancellationToken cancellation = default);
}