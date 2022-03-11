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
public class GenericApiTests : PsbTestContext
{
    public IPsbGenericApi GenericApi => GetRequiredService<IPsbGenericApi>();


    [TestMethod]
    [DeploymentItem("TestData/bisv3.xml", "TestData")]
    public async Task ReceiveFileStreamTest()
    {
        // Arrange
        FileContent file = File.OpenRead("TestData/bisv3.xml");
        var expectedId = Guid.NewGuid().ToString();
        SetAccessToken();
        Configure(builder =>
        {
            var json = "{ \"id\": \"" + expectedId + "\"}";

            builder
                .Setup(HttpMethod.Post, "/api/v1-beta/NL%3aKVK%3aRECEIVER/generic/receive?topic=SalesInvoiceReceived", ensureFileUpload: true)
                .Result(json);
        });

        // Act
        var res = await GenericApi.Receive("NL:KVK:RECEIVER", file, topic: "SalesInvoiceReceived");

        // Assert
        Assert.AreEqual(expectedId, res.Id);
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

            builder
                .Setup(HttpMethod.Post, "/api/v1-beta/NL%3aKVK%3aSENDER/generic/send?topic=SalesInvoiceReceived", ensureFileUpload: true)
                .Result(json);
        });

        // Act
        var res = await GenericApi.Send("NL:KVK:SENDER", file, topic: "SalesInvoiceReceived");

        // Assert
        Assert.AreEqual(expectedId, res.Id);
    }

    [TestMethod]
    [DeploymentItem("TestData/bisv3.xml", "TestData")]
    public async Task DownloadTest()
    {
        // Arrange
        var xml = await File.ReadAllTextAsync("TestData/bisv3.xml");
        var docId = "69fb5e02-7c51-4b62-a469-424054674c4a";
        var partyId = "NL:KVK:12345678";
        var fileFormat = "xml";

        SetAccessToken();
        Configure(builder =>
        {
            var encodedDocumentId = HttpUtility.UrlEncode(docId);
            var encodedPartyId = HttpUtility.UrlEncode(partyId);

            builder
                .Setup(HttpMethod.Get, $"/api/v1-beta/{encodedPartyId}/generic/{encodedDocumentId}/download?targetFormat=xml")
                .Result(xml, contentType: "xml");
        });

        // Act
        var res = await GenericApi.Download(partyId, docId, fileFormat);

        // Assert
        Assert.IsNotNull(res);
        Assert.AreEqual(xml, await res.Content.ReadAsStringAsync());
    }

}