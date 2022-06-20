using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using EConnect.Psb.Api;
using EConnect.Psb.Client;
using EConnect.Psb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EConnect.Psb.UnitTests.Client;

[TestClass]
public class PsbClientTests : PsbTestContext
{
    public IPsbMeApi MeApi => GetRequiredService<IPsbMeApi>();

    [TestMethod]
    public async Task UnexpectedResponseShouldThrow()
    {

        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            builder
                .Setup(HttpMethod.Get, "/api/v1/me")
                .Result("Server Error", HttpStatusCode.InternalServerError, "html");
        });

        // Assert
        var ex = await Assert.ThrowsExceptionAsync<EConnectException>(async () =>
        {
            // Act
            await MeApi.Me();
        });

        Assert.AreEqual("PSB500.JSON02", ex.Code);
        Assert.AreEqual("'Internal Server Error', unexpected JSON ''S' is an invalid start of a value. Path: $ | LineNumber: 0 | BytePositionInLine: 0.'. The message from the server: 'Server Error'.", ex.Message);
    }

    [TestMethod]
    public async Task ErrorResponseShouldThrow()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = @"{
                      ""helpLink"": ""https://psb.econnect.eu/introduction/authentication.html#API403"",
                      ""message"": ""Access forbidden. Please check PartyId authorization and make sure the Subscription-Key is valid."",
                      ""code"": ""API403"",
                      ""requestId"": ""0128d5a881606953f147b08b211826d1"",
                      ""dateTime"": ""2022-02-26T21:59:43.8217536+00:00""
                    }";
            builder
                .Setup(HttpMethod.Get, "/api/v1/me")
                .Result(json, HttpStatusCode.Forbidden);
        });

        // Assert
        var ex = await Assert.ThrowsExceptionAsync<EConnectException>(async () =>
        {
            // Act
            await MeApi.Me();
        });

        Assert.AreEqual("API403", ex.Code);
        Assert.AreEqual("Access forbidden. Please check PartyId authorization and make sure the Subscription-Key is valid.", ex.Message);
    }

    private PsbClient PsbClient => GetRequiredService<PsbClient>();

    public async Task AssertPostFile(string expectedXml, FileContent file)
    {
        // Arrange
        var expectedId = Guid.NewGuid().ToString();
        SetAccessToken();
        Configure(builder =>
        {
            var json = "{ \"id\": \"" + expectedId + "\"}";

            builder
                .Setup(HttpMethod.Post, "/upload", message =>
                {
                    if (message.Content is MultipartFormDataContent content)
                    {
                        var xml = content.ReadAsStringAsync().GetAwaiter().GetResult();
                        Assert.IsTrue(xml.Contains(expectedXml));
                        return true;
                    }

                    return false;
                })
                .Result(json);
        });

        // Act
        var res = await PsbClient.PostFile<Document>("/upload", file);

        // Assert
        Assert.AreEqual(expectedId, res.Id);
    }

    [TestMethod]
    [DeploymentItem("TestData/bisv3.xml", "TestData")]
    public async Task StreamPostFileTest()
    {
        var xmlString = await File.ReadAllTextAsync("TestData/bisv3.xml");
        using FileContent file = new FileContent(File.OpenRead("TestData/bisv3.xml") as Stream, "bisv3");

        await AssertPostFile(xmlString, file);
    }

    [TestMethod]
    [DeploymentItem("TestData/bisv3.xml", "TestData")]
    public async Task FileStreamPostFileTest()
    {
        var xmlString = await File.ReadAllTextAsync("TestData/bisv3.xml");
        using FileContent file = new FileContent(File.OpenRead("TestData/bisv3.xml"), "bisv3");

        await AssertPostFile(xmlString, file);
    }

    [TestMethod]
    [DeploymentItem("TestData/bisv3.xml", "TestData")]
    public async Task BytesPostFileTest()
    {
        var xmlString = await File.ReadAllTextAsync("TestData/bisv3.xml");
        using FileContent file = new FileContent(await File.ReadAllBytesAsync("TestData/bisv3.xml"), "bisv3");

        await AssertPostFile(xmlString, file);
    }

    [TestMethod]
    [DeploymentItem("TestData/bisv3.xml", "TestData")]
    public async Task XmlStringPostFileTest()
    {
        var xmlString = await File.ReadAllTextAsync("TestData/bisv3.xml");
        var fileContent = new FileContent(xmlString, "bisv3");

        await AssertPostFile(xmlString, fileContent);
    }

    [TestMethod]
    [DeploymentItem("TestData/bisv3.xml", "TestData")]
    public async Task XDocumentPostFileTest()
    {
        var xmlString = await File.ReadAllTextAsync("TestData/bisv3.xml");
        var xDocument = XDocument.Parse(xmlString);

        Assert.IsNotNull(xDocument.Root);
        await AssertPostFile("<cbc:EndpointID schemeID=\"0106\">12345678</cbc:EndpointID>", new FileContent(xDocument, "bisv3"));
        await AssertPostFile("<cbc:EndpointID schemeID=\"0106\">12345678</cbc:EndpointID>", new FileContent(xDocument.Root, "bisv3"));
    }

    [TestMethod]
    [DeploymentItem("TestData/bisv3.xml", "TestData")]
    public async Task XmlDocumentPostFileTest()
    {
        var xmlString = await File.ReadAllTextAsync("TestData/bisv3.xml");
        var xDocument = new XmlDocument();
        xDocument.LoadXml(xmlString);

        await AssertPostFile("<cbc:EndpointID schemeID=\"0106\">12345678</cbc:EndpointID>", new FileContent(xDocument, "bisv3"));
    }
}