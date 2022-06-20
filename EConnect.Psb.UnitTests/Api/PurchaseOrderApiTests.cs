using System;
using System.IO;
using System.Net.Http;
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
            var encodedPartyId = HttpUtility.UrlEncode(senderPartyId);

            var requestUri = $"/api/v1/{encodedPartyId}/purchaseOrder/send";

            builder
                .Setup(HttpMethod.Post, requestUri, ensureFileUpload: true)
                .Result(json);
        });

        // Act
        var res = await PsbPurchaseOrderApi.Send(
            senderPartyId: senderPartyId,
            receiverId: null,
            file: file
        );

        // Assert
        Assert.AreEqual(expectedId, res.Id);
    }
}