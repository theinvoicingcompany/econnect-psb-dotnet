using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using EConnect.Psb.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EConnect.Psb.UnitTests.Api;

[TestClass]
public class SalesOrderApiTest : PsbTestContext
{
    public IPsbSalesOrderApi SalesOrderApi => GetRequiredService<IPsbSalesOrderApi>();
    
    [TestMethod]
    [DeploymentItem("TestData/bisv3Order.xml", "TestData")]
    public async Task DownloadTest()
    {
        // Arrange
        var partyId = "NL:KVK:12345678";
        var documentId = "69fb5e02-7c51-4b62-a469-424054674c4a";
        var xml = await File.ReadAllTextAsync("TestData/bisv3Order.xml");

        SetAccessToken();
        Configure(builder =>
        {
            var encodedPartyId = HttpUtility.UrlEncode(partyId);
            var encodedDocumentId = HttpUtility.UrlEncode(documentId);

            builder
                .Setup(HttpMethod.Get, $"/api/v1/{encodedPartyId}/salesOrder/{encodedDocumentId}/download")
                .Result(xml, contentType: "xml");
        });

        // Act
        var res = await SalesOrderApi.Download(partyId, documentId);

        // Assert
        Assert.IsNotNull(res);
        Assert.AreEqual(xml, await res.Content.ReadAsStringAsync());
    }

    [TestMethod]
    public async Task DeleteTest()
    {
        // Arrange
        var partyId = "NL:KVK:12345678";
        var documentId = "69fb5e02-7c51-4b62-a469-424054674c4a";
        SetAccessToken();
        Configure(builder =>
        {
            var encodedPartyId = HttpUtility.UrlEncode(partyId);
            var encodedDocumentId = HttpUtility.UrlEncode(documentId);

            builder
                .Setup(HttpMethod.Delete, $"/api/v1/{encodedPartyId}/salesOrder/{encodedDocumentId}")
                .Result("", contentType: "plain");
        });

        // Act
        await SalesOrderApi.Delete(partyId, documentId);
        
        // Assert
        VerifyDeleteRequest(Times.Once());
    }
}