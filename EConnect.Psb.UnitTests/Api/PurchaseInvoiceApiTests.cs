﻿using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using EConnect.Psb.Api;
using EConnect.Psb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EConnect.Psb.UnitTests.Api;

[TestClass]
public class PurchaseInvoiceApiTests : PsbTestContext
{
    public IPsbPurchaseInvoiceApi PurchaseInvoiceApi => GetRequiredService<IPsbPurchaseInvoiceApi>();


    [TestMethod]
    [DeploymentItem("TestData/bisv3.xml", "TestData")]
    public async Task DownloadTest()
    {
        // Arrange
        var xml = await File.ReadAllTextAsync("TestData/bisv3.xml");
        var docId = "69fb5e02-7c51-4b62-a469-424054674c4a";
        var partyId = "NL:KVK:12345678";

        SetAccessToken();
        Configure(builder =>
        {
            var encodedDocumentId = HttpUtility.UrlEncode(docId);
            var encodedPartyId = HttpUtility.UrlEncode(partyId);

            builder
                .Setup(HttpMethod.Get, $"/api/v1/{encodedPartyId}/purchaseInvoice/{encodedDocumentId}/download")
                .Result(xml, contentType: "xml");
        });

        // Act
        var res = await PurchaseInvoiceApi.Download(partyId, docId);

        // Assert
        Assert.IsNotNull(res);
        Assert.AreEqual(xml, await res.Content.ReadAsStringAsync());
    }

    [TestMethod]
    public async Task ResponseTest()
    {
        // Arrange
        var responseDocId = "e3d1d649-3544-4dac-aa50-10034a1ed168";
        var docId = "69fb5e02-7c51-4b62-a469-424054674c4a";
        var partyId = "NL:KVK:12345678";
        var response = new InvoiceResponse(
            Status: "RE",
            Reasons: new Dictionary<string, string>()
            {
                { "REF", "Purchase order number is invalid. The format should be POnnnnnn." }
            },
            Actions: new Dictionary<string, string>()
            {
                {"NIN", "lease send a new invoice."}
            },
            Note: "Invoice rejected due to validation errors."
        );
        SetAccessToken();
        Configure(builder =>
        {
            var json = $"{{\r\n  \"id\": \"{responseDocId}\"\r\n}}";
            var encodedDocumentId = HttpUtility.UrlEncode(docId);
            var encodedPartyId = HttpUtility.UrlEncode(partyId);

            builder
                .Setup(HttpMethod.Post, $"/api/v1/{encodedPartyId}/purchaseInvoice/{encodedDocumentId}/response",
                    ensureEConnectDocumentId: true, 
                    ensureEConnectDomainId: true)
                .Result(json);
        });

        // Act
        var res = await PurchaseInvoiceApi.Response(partyId, docId, response, responseDocId, "domain1", CancellationToken.None).ConfigureAwait(false);

        // Assert
        Assert.AreEqual(responseDocId, res.Id);
    }

    [TestMethod]
    public async Task DeleteTest()
    {
        // Arrange
        var docId = "69fb5e02-7c51-4b62-a469-424054674c4a";
        var partyId = "NL:KVK:12345678";
        SetAccessToken();
        Configure(builder =>
        {
            var encodedDocumentId = HttpUtility.UrlEncode(docId);
            var encodedPartyId = HttpUtility.UrlEncode(partyId);

            builder
                .Setup(HttpMethod.Delete, $"/api/v1/{encodedPartyId}/purchaseInvoice/{encodedDocumentId}")
                .Result("", contentType: "plain");
        });

        // Act
        await PurchaseInvoiceApi.Delete(partyId, docId);

        // Assert
        VerifyDeleteRequest(Times.Once());
    }

    [TestMethod]
    [DeploymentItem("TestData/example.pdf", "TestData")]
    public async Task RecognizeTest()
    {
        // Arrange
        using FileContent pdf = new(File.OpenRead("TestData/example.pdf"), "pdf");
        var docId = "69fb5e02-7c51-4b62-a469-424054674c4a";
        var partyId = "NL:KVK:12345678";

        SetAccessToken();
        Configure(builder =>
        {
            var json = "{\r\n  \"id\": \"" + docId + "\"\r\n}";
            var encodedPartyId = HttpUtility.UrlEncode(partyId);

            builder
                .Setup(HttpMethod.Post, $"/api/v1/{encodedPartyId}/purchaseInvoice/recognize?channel=idr",
                    ensureEConnectDocumentId: true,
                    ensureEConnectDomainId: true)
                .Result(json);
        });

        // Act
        var res = await PurchaseInvoiceApi.Recognize(partyId, pdf, "idr", docId, "domain1", CancellationToken.None);

        // Assert
        Assert.IsNotNull(res);
        Assert.AreEqual(docId, res.Id);
    }
}