using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EConnect.Psb.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EConnect.Psb.UnitTests.Api;

[TestClass]
public class SalesInvoiceApiTests : PsbTestContext
{
    public IPsbSalesInvoiceApi SalesInvoiceApi => GetRequiredService<IPsbSalesInvoiceApi>();

    [TestMethod]
    public async Task QueryRecipientPartyTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = @"{
                  ""id"": ""NL:KVK:12345678""
                }";

            builder
                .Setup(HttpMethod.Post, "/api/v1/NL%3aKVK%3aSENDER/salesInvoice/queryRecipientParty")
                .Result(json);
        });

        // Act
        var advisedPartyId = await SalesInvoiceApi.QueryRecipientParty("NL:KVK:SENDER", new[] {"NL:KVK:RECEIVER_1", "GLN:RECEIVER_GLN"});

        // Assert
        Assert.AreEqual("NL:KVK:12345678", advisedPartyId);
    }
}