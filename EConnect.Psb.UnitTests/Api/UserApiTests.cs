using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using EConnect.Psb.Api;
using EConnect.Psb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EConnect.Psb.UnitTests.Api;

[TestClass]
public class UserApiTests : PsbTestContext
{
    public IPsbUserApi UserApi => GetRequiredService<IPsbUserApi>();

    [TestMethod]
    public async Task GetUsersTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = @"[
                         {
                           ""name"": ""user1""
                         },
                         {
                           ""name"": ""user2""
                         }]";

            builder
                .Setup(HttpMethod.Get, "/api/v1/user")
                .Result(json);
        });

        // Act
        var res = await UserApi.GetUsers();

        Assert.IsNotNull(res);
        Assert.AreEqual(2, res.Length);
        Assert.AreEqual("user1", res[0].Name);
        Assert.IsTrue(res.All(u => u.Password == null));
    }

    [TestMethod]
    public async Task SetUserTest()
    {
        // Arrange
        var user = new User("user1", "password123");

        SetAccessToken();
        Configure(builder =>
        {
            var json = @"{
                           ""name"": ""user1""
                         }";

            builder
                .Setup(HttpMethod.Put, "/api/v1/user")
                .Result(json);
        });

        // Act
        var res = await UserApi.SetUser(user);

        // Assert
        Assert.IsNotNull(res);
        Assert.AreEqual(user.Name, res.Name);
    }

    [TestMethod]
    public async Task DeleteUserTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            builder
                .Setup(HttpMethod.Delete, "/api/v1/user/user1")
                .Result("", contentType: "plain");
        });

        // Act
        await UserApi.DeleteUser("user1");

        // Assert
        VerifyDeleteRequest(Times.Once());
    }

    [TestMethod]
    public async Task GetUserPartiesTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = @"[
                           {
                              ""name"": ""party1"",
                              ""permissions"": {
                                ""canSendDocument"": true,
                                ""canReceiveDocument"": false,
                                ""canRemoveDocument"": true,
                                ""canManageHook"": false,
                                ""canChangeDocumentStatus"": true
                             },
                             ""id"": ""0106:123""
                           },
                           {
                             ""name"": ""party2"",
                             ""permissions"": {
                               ""canSendDocument"": false,
                               ""canReceiveDocument"": false,
                               ""canRemoveDocument"": false,
                               ""canManageHook"": false,
                               ""canChangeDocumentStatus"": false
                             },
                             ""id"": ""0106:456""
                           }]";

            builder
                .Setup(HttpMethod.Get, "/api/v1/user/username/party")
                .Result(json);
        });

        // Act
        var res = await UserApi.GetUserParties("username");

        // Assert
        Assert.IsNotNull(res);
        Assert.AreEqual(2, res.Length);

        var first = res[0];
        Assert.AreEqual("0106:123", first.Id);
        Assert.AreEqual("party1", first.Name);
        Assert.IsTrue(first.Permissions.CanSendDocument);
        Assert.IsFalse(first.Permissions.CanReceiveDocument);
        Assert.IsTrue(first.Permissions.CanRemoveDocument);
        Assert.IsFalse(first.Permissions.CanManageHook);
        Assert.IsTrue(first.Permissions.CanChangeDocumentStatus);
    }

    [TestMethod]
    public async Task SetUserPartyTest()
    {
        // Arrange
        var user = new UserParty("0106:123", "party1", new PartyPermissions(true, false, true, false, true));

        SetAccessToken();
        Configure(builder =>
        {
            var json = @"  {
                              ""name"": ""party1"",
                              ""permissions"": {
                                ""canSendDocument"": true,
                                ""canReceiveDocument"": false,
                                ""canRemoveDocument"": true,
                                ""canManageHook"": false,
                                ""canChangeDocumentStatus"": true
                             },
                             ""id"": ""0106:123""
                           }";

            builder
                .Setup(HttpMethod.Put, "/api/v1/user/username/party")
                .Result(json);
        });

        // Act
        var res = await UserApi.SetUserParty("username", user);

        // Assert
        Assert.IsNotNull(res);
        Assert.AreEqual("0106:123", res.Id);
        Assert.AreEqual("party1", res.Name);
        Assert.IsTrue(res.Permissions.CanSendDocument);
        Assert.IsFalse(res.Permissions.CanReceiveDocument);
        Assert.IsTrue(res.Permissions.CanRemoveDocument);
        Assert.IsFalse(res.Permissions.CanManageHook);
        Assert.IsTrue(res.Permissions.CanChangeDocumentStatus);
    }

    [TestMethod]
    public async Task DeleteUserPartyTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var encodedParty = HttpUtility.UrlEncode("0106:123");

            builder
                .Setup(HttpMethod.Delete, $"/api/v1/user/username/party/{encodedParty}")
                .Result("", contentType: "plain");
        });

        // Act
        await UserApi.DeleteUserParty("username", "0106:123");

        // Assert
        VerifyDeleteRequest(Times.Once());
    }
}