using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using EConnect.Psb.Api;
using EConnect.Psb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EConnect.Psb.UnitTests.Api;

[TestClass]
public class PurchaseOrderApiTests : PsbTestContext
{
    public IPsbPurchaseOrderApi PsbPurchaseOrderApi => GetRequiredService<IPsbPurchaseOrderApi>();

    [TestMethod]
    public async Task QueryRecipientPartyTest()
    {
        // Arrange
        var expectedId = Guid.NewGuid().ToString();
        var senderPartyId = "NL:KVK:SENDER";

        SetAccessToken();
        Configure(builder =>
        {
            var json = "{\r\n  \"id\": \"" + expectedId + "\"\r\n}";
            var encodedPartyId = HttpUtility.UrlEncode(senderPartyId);

            var requestUri = $"/api/v1/{encodedPartyId}/purchaseOrder/queryRecipientParty";

            builder
                .Setup(HttpMethod.Post, requestUri)
                .Result(json);
        });

        // Act
        var advisedParty = await PsbPurchaseOrderApi.QueryRecipientParty(
            senderPartyId: senderPartyId,
            recipientPartyIds: new[] { "NL:KVK:RECEIVER_1", "GLN:RECEIVER_GLN" },
            preferredDocumentTypeId: null
        );

        // Assert
        Assert.AreEqual(expectedId, advisedParty.Id);
    }

    [TestMethod]
    [DeploymentItem("TestData/bisv3.xml", "TestData")]
    public async Task SendFileStreamTest()
    {
        // Arrange
        var senderPartyId = "NL:KVK:SENDER";
        FileContent file = new(File.OpenRead("TestData/bisv3.xml"), "bisv3");
        var expectedId = Guid.NewGuid().ToString();

        SetAccessToken();
        Configure(builder =>
        {
            var json = "{ \"id\": \"" + expectedId + "\"}";
            var requestUri = $"/api/v1/NL%3aKVK%3aSENDER/purchaseOrder/send?receiverId=NL%3AKVK%3ARECEIVER&channel=peppol";

            builder
                .Setup(HttpMethod.Post, requestUri, 
                    ensureFileUpload: true, 
                    ensureEConnectDocumentId: true,
                    ensureEConnectDomainId: true)
                .Result(json);
        });

        // Act
        var res = await PsbPurchaseOrderApi.Send(
            partyId: senderPartyId, file, "NL:KVK:RECEIVER", "peppol", expectedId, "domain1", CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedId, res.Id);
    }

    [TestMethod]
    public async Task CancelTest()
    {
        // Arrange
        var docId = "69fb5e02-7c51-4b62-a469-424054674c4a";
        var expectedDocId = "e3d1d649-3544-4dac-aa50-10034a1ed168";
        var partyId = "NL:KVK:12345678";
        var response = new OrderCancellation(
            CancellationNote: "The substituted items does not fit our usage.",
            Note: "Cancellation discussed in phone call."
        );
        SetAccessToken();
        Configure(builder =>
        {
            var json = $"{{\r\n  \"id\": \"{expectedDocId}\"\r\n}}";
            var encodedDocumentId = HttpUtility.UrlEncode(docId);
            var encodedPartyId = HttpUtility.UrlEncode(partyId);

            builder
                .Setup(HttpMethod.Post, $"/api/v1/{encodedPartyId}/purchaseOrder/{encodedDocumentId}/cancel",
                    ensureEConnectDocumentId: true, 
                    ensureEConnectDomainId: true)
                .Result(json);
        });

        // Act
        var res = await PsbPurchaseOrderApi.Cancel(partyId, docId, response, expectedDocId, "domain1", CancellationToken.None).ConfigureAwait(false);

        // Assert
        Assert.AreEqual(expectedDocId, res.Id);
    }
}