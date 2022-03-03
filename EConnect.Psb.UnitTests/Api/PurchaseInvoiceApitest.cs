using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using EConnect.Psb.Api;
using EConnect.Psb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EConnect.Psb.UnitTests.Api;

[TestClass]
public class PurchaseInvoiceApitest : PsbTestContext
{
    public IPsbPurchaseInvoiceApi PurchaseInvoiceApi => GetRequiredService<IPsbPurchaseInvoiceApi>();

    private readonly InvoiceResponse ExampleInvoiceResponse = new InvoiceResponse(
        Status: "AP",
        Reasons: new string[] { "example" },
        Actions: new string[] { "example" },
        Note: "1",
        CreatedOn: DateTime.Parse("2022-02-26T21:59:43.8217536+00:00"),
        ChangedOn: DateTime.Parse("2022-02-26T21:59:43.8217536+00:00")
    );

    private readonly string ExamplePartyId = "NL:KVK:12345678";

    private readonly string ExampleDocumentId = "1";

    private readonly string FakeFile = "foo.bar";


    [TestMethod]
    public async Task DownloadTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = "{\r\n  \"id\": \"" + ExampleDocumentId + "\"\r\n}";
            var encodedDocumentId = HttpUtility.UrlEncode(ExampleDocumentId);
            var encodedPartyId = HttpUtility.UrlEncode(ExamplePartyId);

            builder
                .Setup(HttpMethod.Get, $"/api/v1/{encodedPartyId}/purchaseInvoice/{encodedDocumentId}/download")
                .Result(json);
        });

        // Act
        var res = await PurchaseInvoiceApi.Download(ExamplePartyId, ExampleDocumentId);

        // Assert
        Assert.IsNotNull(res);
        Assert.AreEqual(ExampleDocumentId, res);
    }

    [TestMethod]
    public void SendTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = "{\r\n  \"Id\": \"" + ExampleDocumentId + "\"\r\n}";
            var encodedDocumentId = HttpUtility.UrlEncode(ExampleDocumentId);
            var encodedPartyId = HttpUtility.UrlEncode(ExamplePartyId);

            builder
                .Setup(HttpMethod.Post, $"/api/v1/{encodedPartyId}/purchaseInvoice/{encodedDocumentId}/response")
                .Result(json);
        });

        // Act
        var res = PurchaseInvoiceApi.Response(ExamplePartyId, ExampleDocumentId, ExampleInvoiceResponse).Result;

        // Assert
        Assert.AreEqual(ExampleDocumentId, res.Id);
    }

    [TestMethod]
    public async Task DeleteTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = "{ " +
                "\"Status\":\"AP\"," +
                "\"Reasons\":[\"example\"]," +
                "\"Actions\":[\"example\"]," +
                "\"Note\":\"1\"," +
                "\"CreatedOn\":\"2022-02-26T22:59:43.8217536+01:00\"," +
                "\"ChangedOn\":\"2022-02-26T22:59:43.8217536+01:00\"" +
            "}";
    var encodedDocumentId = HttpUtility.UrlEncode(ExampleDocumentId);
            var encodedPartyId = HttpUtility.UrlEncode(ExamplePartyId);

            builder
                .Setup(HttpMethod.Delete, $"/api/v1/{encodedPartyId}/purchaseInvoice/{encodedDocumentId}")
                .Result(json);
        });

        // Act
        PurchaseInvoiceApi.Delete(ExamplePartyId, ExampleDocumentId);

        // Assert
        // Implicitly succeeded
    }

    [TestMethod]
    public async Task RecognizeTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = "{\r\n  \"Id\": \"" + ExamplePartyId + "\"\r\n}";
            var encodedPartyId = HttpUtility.UrlEncode(ExamplePartyId);

            builder
                .Setup(HttpMethod.Post, $"/api/v1/{encodedPartyId}/purchaseInvoice/recognize")
                .Result(json);
        });

        // Act
        var res = await PurchaseInvoiceApi.Recognize(ExamplePartyId, FakeFile);

        // Assert
        Assert.IsNotNull(res);
        Assert.AreEqual(ExamplePartyId, res.Id);
    }
}