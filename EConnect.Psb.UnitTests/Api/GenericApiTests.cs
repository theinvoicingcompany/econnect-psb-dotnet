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
public class GenericApiTests : PsbTestContext
{
    public IPsbGenericApi GenericApi => GetRequiredService<IPsbGenericApi>();


    [TestMethod]
    [DeploymentItem("TestData/bisv3.xml", "TestData")]
    public async Task ReceiveFileStreamTest()
    {
        // Arrange
        FileContent file = new(File.OpenRead("TestData/bisv3.xml"), "bisv3");
        var expectedId = Guid.NewGuid().ToString();

        SetAccessToken();
        Configure(builder =>
        {
            var json = "{ \"id\": \"" + expectedId + "\"}";

            builder
                .Setup(HttpMethod.Post, "/api/v1-beta/NL%3aKVK%3aRECEIVER/generic/receive?senderId=NL%3AKVK%3ASENDER&topic=SalesInvoiceReceived&channel=peppol&sourceFormat=urn%3Acen.eu%3Aen16931%3A2017%23compliant%23urn%3Afdc%3Apeppol.eu%3A2017%3Apoacc%3Abilling%3A3.0&targetFormat=urn%3Acen.eu%3Aen16931%3A2017%23compliant%23urn%3Afdc%3Anen.nl%3Anlcius%3Av1.0%23conformant%23urn%3Afdc%3Anen.nl%3Agaccount%3Av1.0",
                    ensureFileUpload: true,
                    ensureEConnectDocumentId: true)
                .Result(json);
        });

        // Act
        var res = await GenericApi.Receive(
            "NL:KVK:RECEIVER",
            file,
            "SalesInvoiceReceived",
            "NL:KVK:SENDER",
            "peppol",
            false,
            expectedId,
            "urn:cen.eu:en16931:2017#compliant#urn:fdc:peppol.eu:2017:poacc:billing:3.0",
            "urn:cen.eu:en16931:2017#compliant#urn:fdc:nen.nl:nlcius:v1.0#conformant#urn:fdc:nen.nl:gaccount:v1.0",
            CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedId, res.Id);
    }

    [TestMethod]
    [DeploymentItem("TestData/bisv3.xml", "TestData")]
    public async Task SendFileStreamTest()
    {
        // Arrange
        FileContent file = new(File.OpenRead("TestData/bisv3.xml"), "bisv3");
        var expectedId = Guid.NewGuid().ToString();

        SetAccessToken();
        Configure(builder =>
        {
            var json = "{ \"id\": \"" + expectedId + "\"}";

            builder
                .Setup(HttpMethod.Post, "/api/v1-beta/NL%3aKVK%3aSENDER/generic/send?receiverId=NL%3AKVK%3ARECEIVER&topic=SalesInvoiceReceived&channel=peppol&disablePlugins=True&sourceFormat=urn%3Acen.eu%3Aen16931%3A2017%23compliant%23urn%3Afdc%3Apeppol.eu%3A2017%3Apoacc%3Abilling%3A3.0&targetFormat=urn%3Acen.eu%3Aen16931%3A2017%23compliant%23urn%3Afdc%3Anen.nl%3Anlcius%3Av1.0%23conformant%23urn%3Afdc%3Anen.nl%3Agaccount%3Av1.0",
                    ensureFileUpload: true,
                    ensureEConnectDocumentId: true)
                .Result(json);
        });

        // Act
        var res = await GenericApi.Send(
            "NL:KVK:SENDER",
            file,
            "SalesInvoiceReceived",
            "NL:KVK:RECEIVER",
            "peppol",
            true,
            expectedId,
            "urn:cen.eu:en16931:2017#compliant#urn:fdc:peppol.eu:2017:poacc:billing:3.0",
            "urn:cen.eu:en16931:2017#compliant#urn:fdc:nen.nl:nlcius:v1.0#conformant#urn:fdc:nen.nl:gaccount:v1.0",
            CancellationToken.None);

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