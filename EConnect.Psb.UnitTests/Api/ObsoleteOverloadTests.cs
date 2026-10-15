using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EConnect.Psb.Api;
using EConnect.Psb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EConnect.Psb.UnitTests.Api;

/// <summary>
/// The pre-v1-update signatures are kept, marked obsolete, so callers compiled against the previous
/// package keep working. Each must forward to its replacement and send the same request.
/// </summary>
[TestClass]
#pragma warning disable CS0618 // These tests exercise the obsolete overloads on purpose.
public class ObsoleteOverloadTests : PsbTestContext
{
    public IPsbGenericApi GenericApi => GetRequiredService<IPsbGenericApi>();
    public IPsbPurchaseInvoiceApi PurchaseInvoiceApi => GetRequiredService<IPsbPurchaseInvoiceApi>();
    public IPsbSalesOrderApi SalesOrderApi => GetRequiredService<IPsbSalesOrderApi>();
    public IPsbPurchaseOrderApi PurchaseOrderApi => GetRequiredService<IPsbPurchaseOrderApi>();

    [TestMethod]
    [DeploymentItem("TestData/bisv3.xml", "TestData")]
    public async Task GenericReceive_ObsoleteOverload_ForwardsAsOptions()
    {
        // Arrange
        FileContent file = new(File.OpenRead("TestData/bisv3.xml"), "bisv3");
        var expectedId = Guid.NewGuid().ToString();

        SetAccessToken();
        Configure(builder =>
        {
            builder
                .Setup(HttpMethod.Post, "/api/v1/NL%3aKVK%3aRECEIVER/generic/receive?senderId=NL%3AKVK%3ASENDER&topic=SalesInvoiceReceived&channel=peppol&disablePlugins=True&sourceFormat=src&targetFormat=tgt",
                    ensureFileUpload: true,
                    ensureMetaAttributes: true,
                    ensureEConnectDocumentId: true,
                    ensureEConnectDomainId: true)
                .Result("{ \"id\": \"" + expectedId + "\"}");
        });

        // Act
        var res = await GenericApi.Receive(
            "NL:KVK:RECEIVER",
            file,
            "SalesInvoiceReceived",
            "NL:KVK:SENDER",
            "peppol",
            true,
            expectedId,
            "src",
            "tgt",
            new Dictionary<string, string> { { "key1", "value1" } },
            "domain1",
            CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedId, res.Id);
    }

    [TestMethod]
    [DeploymentItem("TestData/bisv3.xml", "TestData")]
    public async Task GenericSend_ObsoleteOverload_ForwardsAsOptions()
    {
        // Arrange
        FileContent file = new(File.OpenRead("TestData/bisv3.xml"), "bisv3");
        var expectedId = Guid.NewGuid().ToString();

        SetAccessToken();
        Configure(builder =>
        {
            builder
                .Setup(HttpMethod.Post, "/api/v1/NL%3aKVK%3aSENDER/generic/send?receiverId=NL%3AKVK%3ARECEIVER&topic=SalesInvoiceReceived&channel=peppol&sourceFormat=src&targetFormat=tgt",
                    ensureFileUpload: true,
                    ensureMetaAttributes: true,
                    ensureEConnectDocumentId: true,
                    ensureEConnectDomainId: true)
                .Result("{ \"id\": \"" + expectedId + "\"}");
        });

        // Act
        var res = await GenericApi.Send(
            "NL:KVK:SENDER",
            file,
            "SalesInvoiceReceived",
            "NL:KVK:RECEIVER",
            "peppol",
            false,
            expectedId,
            "src",
            "tgt",
            new Dictionary<string, string> { { "key1", "value1" } },
            "domain1",
            CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedId, res.Id);
    }

    [TestMethod]
    public async Task GenericDownload_ObsoleteOverload_Forwards()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            builder
                .Setup(HttpMethod.Get, "/api/v1/NL%3aKVK%3a12345678/generic/doc1/download?targetFormat=pdf",
                    ensureEConnectDomainId: true)
                .Result("pdf", contentType: "plain");
        });

        // Act
        var res = await GenericApi.Download("NL:KVK:12345678", "doc1", "pdf", "domain1", CancellationToken.None);

        // Assert
        Assert.AreEqual("pdf", await res.Content.ReadAsStringAsync());
    }

    [TestMethod]
    public async Task PurchaseInvoiceDownload_ObsoleteOverload_Forwards()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            builder
                .Setup(HttpMethod.Get, "/api/v1/NL%3aKVK%3a12345678/purchaseInvoice/doc1/download",
                    ensureEConnectDomainId: true)
                .Result("xml", contentType: "plain");
        });

        // Act
        var res = await PurchaseInvoiceApi.Download("NL:KVK:12345678", "doc1", "domain1", CancellationToken.None);

        // Assert
        Assert.AreEqual("xml", await res.Content.ReadAsStringAsync());
    }

    [TestMethod]
    public async Task SalesOrderDownload_ObsoleteOverload_Forwards()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            builder
                .Setup(HttpMethod.Get, "/api/v1/NL%3aKVK%3a12345678/salesOrder/doc1/download",
                    ensureEConnectDomainId: true)
                .Result("xml", contentType: "plain");
        });

        // Act
        var res = await SalesOrderApi.Download("NL:KVK:12345678", "doc1", "domain1", CancellationToken.None);

        // Assert
        Assert.AreEqual("xml", await res.Content.ReadAsStringAsync());
    }

    [TestMethod]
    public async Task PurchaseOrderQueryRecipientParty_ObsoleteOverload_Forwards()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            builder
                .Setup(HttpMethod.Post, "/api/v1/NL%3aKVK%3aSENDER/purchaseOrder/queryRecipientParty?preferredDocumentTypeId=doc%23type",
                    ensureEConnectDomainId: true)
                .Result(@"{ ""id"": ""0106:12345678"" }");
        });

        // Act
        var party = await PurchaseOrderApi.QueryRecipientParty(
            "NL:KVK:SENDER", ["NL:KVK:RECEIVER_1"], "doc#type", "domain1", CancellationToken.None);

        // Assert
        Assert.AreEqual("0106:12345678", party.Id);
    }

    [TestMethod]
    public void Party_SingleArgumentConstructor_LeavesOptionalPropertiesNull()
    {
        // Act
        var party = new Party("0106:12345678");

        // Assert
        Assert.AreEqual("0106:12345678", party.Id);
        Assert.IsNull(party.Channel);
        Assert.IsNull(party.Description);
    }

    [TestMethod]
    public void Party_Deserializes_WithTwoPublicConstructors()
    {
        // Act
        var party = JsonSerializer.Deserialize<Party>(
            @"{ ""id"": ""0106:12345678"", ""channel"": ""peppol"", ""description"": ""Main"" }",
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        // Assert
        Assert.AreEqual(new Party("0106:12345678", "peppol", "Main"), party);
    }
}
#pragma warning restore CS0618
