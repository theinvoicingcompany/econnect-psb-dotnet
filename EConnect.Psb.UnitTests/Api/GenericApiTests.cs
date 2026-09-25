using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                    ensureMetaAttributes: true,
                    ensureEConnectDocumentId: true, 
                    ensureEConnectDomainId: true)
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
            new Dictionary<string, string>() { {"key1", "value1"} },
            "domain1",
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
                    ensureMetaAttributes: true,
                    ensureEConnectDocumentId: true,
                    ensureEConnectDomainId: true)
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
            new Dictionary<string, string>() { {"key1", "value1"} },
            "domain1",
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

    [TestMethod]
    public async Task GetValidationOptionsTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = @"[
                         {
                           ""id"": ""validation-id"",
                           ""name"": ""Custom validation"",
                           ""state"": ""inherited => on"",
                           ""createdOn"": ""2025-01-01T12:00:00+00:00""
                         }]";

            builder
                .Setup(HttpMethod.Get, "/api/v1/generic/validate/options", ensureEConnectDomainId: true)
                .Result(json);
        });

        // Act
        var res = await GenericApi.GetValidationOptions("domain1");

        // Assert
        Assert.AreEqual(1, res.Length);
        Assert.AreEqual("validation-id", res[0].Id);
        Assert.AreEqual("Custom validation", res[0].Name);
        Assert.AreEqual("inherited => on", res[0].State);
        Assert.AreEqual(new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero), res[0].CreatedOn);
        Assert.IsNull(res[0].ChangedOn);
    }

    [TestMethod]
    [DeploymentItem("TestData/bisv3.xml", "TestData")]
    public async Task ValidateValidDocumentTest()
    {
        // Arrange
        FileContent file = new(File.OpenRead("TestData/bisv3.xml"), "bisv3");

        SetAccessToken();
        Configure(builder =>
        {
            var json = @"{
                           ""isValid"": true,
                           ""source"": {
                             ""typeId"": ""urn:cen.eu:en16931:2017#compliant#urn:fdc:peppol.eu:2017:poacc:billing:3.0"",
                             ""typeName"": ""Peppol BIS Billing UBL Invoice V3"",
                             ""profileId"": ""urn:fdc:peppol.eu:2017:poacc:billing:01:1.0""
                           },
                           ""assertions"": []
                         }";

            builder
                .Setup(HttpMethod.Post, "/api/v1/generic/validate?sourceFormat=urn%3Acen.eu%3Aen16931%3A2017%23compliant%23urn%3Afdc%3Apeppol.eu%3A2017%3Apoacc%3Abilling%3A3.0&optionId=validation-id",
                    ensureFileUpload: true,
                    ensureMetaAttributes: true,
                    ensureEConnectDomainId: true)
                .Result(json);
        });

        // Act
        var res = await GenericApi.Validate(
            file,
            "urn:cen.eu:en16931:2017#compliant#urn:fdc:peppol.eu:2017:poacc:billing:3.0",
            "validation-id",
            metaAttributes: new Dictionary<string, string>() { { "key1", "value1" } },
            domainId: "domain1",
            cancellation: CancellationToken.None);

        // Assert
        Assert.AreEqual(true, res.IsValid);
        Assert.IsNotNull(res.Source);
        Assert.AreEqual("Peppol BIS Billing UBL Invoice V3", res.Source.TypeName);
        Assert.AreEqual(0, res.Assertions.Length);
    }

    [TestMethod]
    [DeploymentItem("TestData/bisv3.xml", "TestData")]
    public async Task ValidateInvalidDocumentTest()
    {
        // Arrange
        FileContent file = new(File.OpenRead("TestData/bisv3.xml"), "bisv3");

        SetAccessToken();
        Configure(builder =>
        {
            var json = @"{
                           ""isValid"": false,
                           ""source"": {
                             ""typeId"": ""urn:cen.eu:en16931:2017#compliant#urn:fdc:peppol.eu:2017:poacc:billing:3.0""
                           },
                           ""assertions"": [
                             {
                               ""artifact"": { ""type"": ""business"", ""name"": ""Peppol BIS Billing"" },
                               ""flag"": ""fatal"",
                               ""test"": ""cbc:EndpointID"",
                               ""location"": ""/Invoice"",
                               ""code"": ""PEPPOL-EN16931-R020"",
                               ""text"": ""Seller electronic address MUST be provided.""
                             }
                           ]
                         }";

            builder
                .Setup(HttpMethod.Post, "/api/v1/generic/validate", ensureFileUpload: true)
                .Result(json);
        });

        // Act
        var res = await GenericApi.Validate(file);

        // Assert
        Assert.AreEqual(false, res.IsValid);
        Assert.AreEqual("urn:cen.eu:en16931:2017#compliant#urn:fdc:peppol.eu:2017:poacc:billing:3.0", res.Source?.TypeId);
        Assert.AreEqual(1, res.Assertions.Length);

        var assertion = res.Assertions[0];
        Assert.AreEqual("business", assertion.Artifact.Type);
        Assert.AreEqual("Peppol BIS Billing", assertion.Artifact.Name);
        Assert.AreEqual("fatal", assertion.Flag);
        Assert.AreEqual("cbc:EndpointID", assertion.Test);
        Assert.AreEqual("/Invoice", assertion.Location);
        Assert.AreEqual("PEPPOL-EN16931-R020", assertion.Code);
        Assert.AreEqual("Seller electronic address MUST be provided.", assertion.Text);
    }

    [TestMethod]
    [DeploymentItem("TestData/bisv3.xml", "TestData")]
    public async Task ValidateWithLanguageHeaderTest()
    {
        // Arrange
        FileContent file = new(File.OpenRead("TestData/bisv3.xml"), "bisv3");

        SetAccessToken();
        Configure(builder =>
        {
            builder
                .Setup(HttpMethod.Post, "/api/v1/generic/validate", message =>
                    message.Headers.TryGetValues("X-EConnect-Language", out var values) &&
                    values.Single() == "nl")
                .Result(@"{ ""isValid"": true, ""assertions"": [] }");
        });

        // Act
        var res = await GenericApi.Validate(file, languageCode: "nl");

        // Assert
        Assert.AreEqual(true, res.IsValid);
    }
}