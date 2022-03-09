using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
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
}