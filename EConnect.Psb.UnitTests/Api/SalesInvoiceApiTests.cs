using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EConnect.Psb.Api;
using EConnect.Psb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EConnect.Psb.UnitTests.Api;

[TestClass]
public class SalesInvoiceApiTests : PsbTestContext
{
    public IPsbSalesInvoiceApi SalesInvoiceApi => GetRequiredService<IPsbSalesInvoiceApi>();

    [TestMethod]
    public async Task QueryRecipientPartyTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = @"{
                  ""id"": ""NL:KVK:12345678""
                }";

            builder
                .Setup(HttpMethod.Post, "/api/v1/NL%3aKVK%3aSENDER/salesInvoice/queryRecipientParty")
                .Result(json);
        });

        // Act
        var advisedPartyId = await SalesInvoiceApi.QueryRecipientParty("NL:KVK:SENDER", new[] { "NL:KVK:RECEIVER_1", "GLN:RECEIVER_GLN" });

        // Assert
        Assert.AreEqual("NL:KVK:12345678", advisedPartyId);
    }

    [TestMethod]
    [DeploymentItem("TestData/bisv3.xml", "TestData")]
    public async Task SendFileStreamTest()
    {
        // Arrange
        FileContent file = new FileContent(File.OpenRead("TestData/bisv3.xml"), "bisv3");
        var expectedId = Guid.NewGuid().ToString();

        SetAccessToken();
        Configure(builder =>
        {
            var json = "{ \"id\": \"" + expectedId + "\"}";

            builder
                .Setup(HttpMethod.Post, "/api/v1/NL%3aKVK%3aSENDER/salesInvoice/send?receiverId=NL%3AKVK%3ARECEIVER&channel=peppol",
                    ensureFileUpload: true,
                    ensureEConnectDocumentId: true,
                    ensureEConnectDomainId: true)
                .Result(json);
        });

        // Act
        var res = await SalesInvoiceApi.Send("NL:KVK:SENDER", file, "NL:KVK:RECEIVER",
            "peppol", expectedId, "domain1", CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedId, res.Id);
    }

    [TestMethod]
    [DeploymentItem("TestData/example.pdf", "TestData")]
    public async Task RecognizeFileStreamTest()
    {
        // Arrange
        FileContent file = new FileContent(File.OpenRead("TestData/example.pdf"), "bisv3");
        var expectedId = Guid.NewGuid().ToString();

        SetAccessToken();
        Configure(builder =>
        {
            var json = "{ \"id\": \"" + expectedId + "\"}";

            builder
                .Setup(HttpMethod.Post, "/api/v1/NL%3aKVK%3aSENDER/salesInvoice/recognize?channel=idr", 
                    ensureFileUpload: true,
                    ensureEConnectDocumentId: true,
                    ensureEConnectDomainId: true)
                .Result(json);
        });

        // Act
        var res = await SalesInvoiceApi.Recognize("NL:KVK:SENDER", file, "idr",
            expectedId, "domain1", CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedId, res.Id);
    }
}