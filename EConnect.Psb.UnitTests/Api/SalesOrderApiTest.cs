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
public class SalesOrderApiTest : PsbTestContext
{
    public IPsbSalesOrderApi SalesOrderApi => GetRequiredService<IPsbSalesOrderApi>();

    private readonly string ExamplePartyId = "NL:KVK:12345678";
    private readonly string ExampleDocumentId = "69fb5e02-7c51-4b62-a469-424054674c4a";

    private readonly DocumentStatus _exampleDocumentStatus = new DocumentStatus(
        Id: "1",
        Name: "example status",
        Description: "example description",
        CreatedOn: DateTime.Parse("2022-03-03T14:42:40.142Z")
        );

    [TestMethod]
    [DeploymentItem("TestData/bisv3Order.xml", "TestData")]
    public async Task DownloadTest()
    {
        // Arrange
        var xml = await File.ReadAllTextAsync("TestData/bisv3Order.xml");

        SetAccessToken();
        Configure(builder =>
        {
            var encodedPartyId = HttpUtility.UrlEncode(ExamplePartyId);
            var encodedDocumentId = HttpUtility.UrlEncode(ExampleDocumentId);

            builder
                .Setup(HttpMethod.Get, $"/api/v1/{encodedPartyId}/salesOrder/{encodedDocumentId}/download")
                .Result(xml, contentType: "xml");
        });

        // Act
        var res = await SalesOrderApi.Download(ExamplePartyId, ExampleDocumentId);

        // Assert
        Assert.IsNotNull(res);
        Assert.AreEqual(xml, await res.Content.ReadAsStringAsync());
    }

    [TestMethod]
    public async Task DeleteTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = @"{
                ""id"": ""69fb5e02-7c51-4b62-a469-424054674c4a"",
                ""name"": ""example status"",
                ""description"": ""example description"",
                ""createdOn"": ""2022-03-03T14:42:40.142Z""
            }";

            var encodedPartyId = HttpUtility.UrlEncode(ExamplePartyId);
            var encodedDocumentId = HttpUtility.UrlEncode(ExampleDocumentId);

            builder
                .Setup(HttpMethod.Delete, $"/api/v1/{encodedPartyId}/salesOrder/{encodedDocumentId}")
                .Result(json);
        });

        // Act
        await SalesOrderApi.Delete(
            partyId: ExamplePartyId,
            documentId: ExampleDocumentId
        );

        // Assert
        // Implicitly succeeded
    }
}