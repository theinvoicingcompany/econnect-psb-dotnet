using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using EConnect.Psb.Client;
using EConnect.Psb.Models;

namespace EConnect.Psb.Api;

public class PsbSalesInvoiceApi : IPsbSalesInvoiceApi
{
    private readonly PsbClient _psbClient;

    public PsbSalesInvoiceApi(PsbClient psbClient)
    {
        _psbClient = psbClient;
    }
    public async Task<string> QueryRecipientParty(string senderPartyId, string[] recipientPartyIds, CancellationToken cancellation = default)
    {
        var party = await _psbClient.Post<Party>($"/api/v1/{HttpUtility.UrlEncode(senderPartyId)}/salesInvoice/queryRecipientParty", recipientPartyIds, cancellation);
        return party.Id;
    }
}