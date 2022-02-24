using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EConnect.Psb.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EConnect.Psb.UnitTests.Api;

[TestClass]
public class MeApiTests : PsbTestContext
{   
    public IPsbMeApi MeApi => GetRequiredService<IPsbMeApi>();

    [TestMethod]
    public async Task Me()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = "{\r\n  \"name\": \"eConnectUser\"\r\n}";
            builder
                .Setup(HttpMethod.Get, "/api/v1/me")
                .Result(json);
        });

        // Act
        var res = await MeApi.Me();

        // Assert
        Assert.AreEqual("eConnectUser", res.Name);
    }


    [TestMethod]
    public async Task MeParties()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = @"[
                  {
                    ""id"": ""NL:KVK:12345678"",
                    ""name"": ""Example party"",
                    ""permissions"": {
                      ""canSendDocument"": true,
                      ""canReceiveDocument"": true,
                      ""canRemoveDocument"": true,
                      ""canManageHook"": true,
                      ""canChangeDocumentStatus"": true
                    }
                  }
                ]";
            builder
                .Setup(HttpMethod.Get, "/api/v1/me/party")
                .Result(json);
        });

        // Act
        var res = (await MeApi.MeParties()).ToList();

        // Assert
        var party = res.First();
        Assert.AreEqual(party.Id, "NL:KVK:12345678");
        Assert.AreEqual(party.Name, "Example party");
    }
}