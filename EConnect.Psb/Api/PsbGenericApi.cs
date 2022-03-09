using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using EConnect.Psb.Client;
using EConnect.Psb.Models;

namespace EConnect.Psb.Api;

public class PsbGenericApi : IPsbGenericApi
{
    private readonly PsbClient _psbClient;
    public PsbGenericApi(PsbClient psbClient)
    {
        _psbClient = psbClient;
    }

    public async Task<Document> Receive(string receiverPartyId,
        FileContent file, string? topic = null, string? senderPartyId = null,
        CancellationToken cancellation = default)
    {
        var requestUri = $"/api/v1-beta/{HttpUtility.UrlEncode(receiverPartyId)}/generic/receive";
        var query = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(senderPartyId))
            query.Add("senderId", senderPartyId!);

        if (!string.IsNullOrEmpty(topic))
            query.Add("topic", topic!);

        var queryString = await new FormUrlEncodedContent(query).ReadAsStringAsync();
        if (!string.IsNullOrEmpty(queryString))
            requestUri += "?" + queryString;

        var res = await _psbClient.PostFile<Document>(requestUri, file, cancellation);
        return res;
    }
}