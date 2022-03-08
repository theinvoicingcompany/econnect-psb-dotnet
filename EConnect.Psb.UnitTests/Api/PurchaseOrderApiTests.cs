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

    private readonly string ExampleSenderPartyId = "NL:KVK:SENDER";

    private readonly string ExampleDocumentId = "1";

    [TestMethod]
    public async Task QueryRecipientPartyTest()
    {
        // Arrange
        var expectedId = Guid.NewGuid().ToString();

        SetAccessToken();
        Configure(builder =>
        {
            var json = "{\r\n  \"id\": \"" + expectedId + "\"\r\n}";
            var encodedDocumentId = HttpUtility.UrlEncode(ExampleDocumentId);
            var encodedPartyId = HttpUtility.UrlEncode(ExampleSenderPartyId);

            var requestUri = $"/api/v1/{encodedPartyId}/purchaseOrder/queryRecipientParty";

            builder
                .Setup(HttpMethod.Post, requestUri)
                .Result(json);
        });

        // Act
        var advisedParty = await PsbPurchaseOrderApi.QueryRecipientParty(
            partyId: ExampleSenderPartyId,
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
        FileContent file = File.OpenRead("TestData/bisv3.xml");
        var expectedId = Guid.NewGuid().ToString();

        SetAccessToken();
        Configure(builder =>
        {
            var json = "{ \"id\": \"" + expectedId + "\"}";
            var encodedPartyId = HttpUtility.UrlEncode(ExampleSenderPartyId);

            var requestUri = $"/api/v1/{encodedPartyId}/purchaseOrder/send";

            builder
                .Setup(HttpMethod.Post, requestUri, ensureFileUpload: true)
                .Result(json);
        });

        // Act
        var res = await PsbPurchaseOrderApi.Send(
            partyId: ExampleSenderPartyId,
            receiverId: null,
            file: file
        );

        // Assert
        Assert.AreEqual(expectedId, res.Id);
    }
}