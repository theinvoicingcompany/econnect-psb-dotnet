using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using EConnect.Psb.Api;
using EConnect.Psb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace EConnect.Psb.UnitTests.Api;

[TestClass]
public class HookApiTests : PsbTestContext
{
    public IPsbHookApi HookApi => GetRequiredService<IPsbHookApi>();

    private readonly Hook ExampleHook = new Hook(
        Id: "1",
        Action: "mailto:techsupport@econnect.eu",
        Name: "mail hook",
        Topics: new string[] {
            "*Received",
            "*ReceivedError",
            "*Sent",
            "*SentRetry",
            "*SentError",
            "HookSent",
            "HookSentError",
            "HookSentRetry"
        },
        IsActive: true
    );

    private readonly string ExamplePartyId = "NL:KVK:12345678";

    [TestMethod]
    public async Task GetEnviromentHooksTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = $"[ {JsonConvert.SerializeObject(ExampleHook)} ]";
            builder
                .Setup(HttpMethod.Get, "/api/v1/hook")
                .Result(json);
        });

        // Act
        var res = await HookApi.GetEnviromentHooks();

        // Assert
        Assert.IsNotNull(res);
        Assert.IsNotNull(res[0]);
        Assert.AreEqual(ExampleHook.Id, res[0].Id);
        Assert.AreEqual(ExampleHook.Action, res[0].Action);
        Assert.AreEqual(ExampleHook.Name, res[0].Name);
        for (int i = 0; i < ExampleHook.Topics.Length; i++)
        {
            Assert.AreEqual(ExampleHook.Topics[i], res[0].Topics[i]);
        }
        Assert.AreEqual(ExampleHook.IsActive, res[0].IsActive);
    }

    [TestMethod]
    public async Task PutEnviromentHooksTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = JsonConvert.SerializeObject(ExampleHook);
            builder
                .Setup(HttpMethod.Put, "/api/v1/hook")
                .Result(json);
        });

        // Act
        var res = await HookApi.SetEnviromentHook(ExampleHook);

        // Assert
        Assert.AreEqual(ExampleHook.Id, res.Id);
        Assert.AreEqual(ExampleHook.Action, res.Action);
        Assert.AreEqual(ExampleHook.Name, res.Name);
        for(int i = 0; i < ExampleHook.Topics.Length; i++)
        {
            Assert.AreEqual(ExampleHook.Topics[i], res.Topics[i]);
        }
        Assert.AreEqual(ExampleHook.IsActive, res.IsActive);
    }

    [TestMethod]
    public async Task DeleteDefaultHookTest()
    {
        var targetId = ExampleHook.Id;

        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = JsonConvert.SerializeObject(ExampleHook);
            builder
                .Setup(HttpMethod.Delete, $"/api/v1/hook/{targetId}")
                .Result(json);
        });

        // Act
        await HookApi.DeleteDefaultHook(targetId);

        // Assert
        // Implicitely succeeded
    }



    [TestMethod]
    public async Task GetPartyHooksTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = $"[ {JsonConvert.SerializeObject(ExampleHook)} ]";
            var encodedPartyId = HttpUtility.UrlEncode(ExamplePartyId);

            builder
                .Setup(HttpMethod.Get, $"/api/v1/{encodedPartyId}/hook")
                .Result(json);
        });

        // Act
        var res = await HookApi.GetPartyHooks(ExamplePartyId);

        // Assert
        Assert.IsNotNull(res);
        Assert.IsNotNull(res[0]);
        Assert.AreEqual(ExampleHook.Id, res[0].Id);
        Assert.AreEqual(ExampleHook.Action, res[0].Action);
        Assert.AreEqual(ExampleHook.Name, res[0].Name);
        for (int i = 0; i < ExampleHook.Topics.Length; i++)
        {
            Assert.AreEqual(ExampleHook.Topics[i], res[0].Topics[i]);
        }
        Assert.AreEqual(ExampleHook.IsActive, res[0].IsActive);
    }

    [TestMethod]
    public async Task PutPartyHooksTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = JsonConvert.SerializeObject(ExampleHook);
            var encodedPartyId = HttpUtility.UrlEncode(ExamplePartyId);

            builder
                .Setup(HttpMethod.Put, $"/api/v1/{encodedPartyId}/hook")
                .Result(json);
        });

        // Act
        var res = await HookApi.SetPartyHooks(ExamplePartyId, ExampleHook);

        // Assert
        Assert.AreEqual(ExampleHook.Id, res.Id);
        Assert.AreEqual(ExampleHook.Action, res.Action);
        Assert.AreEqual(ExampleHook.Name, res.Name);
        for (int i = 0; i < ExampleHook.Topics.Length; i++)
        {
            Assert.AreEqual(ExampleHook.Topics[i], res.Topics[i]);
        }
        Assert.AreEqual(ExampleHook.IsActive, res.IsActive);
    }

    [TestMethod]
    public async Task PingPartyHooksTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = "{\r\n  \"id\": \"" + ExamplePartyId +"\"\r\n}";
            var encodedPartyId = HttpUtility.UrlEncode(ExamplePartyId);

            builder
                .Setup(HttpMethod.Get, $"/api/v1/{encodedPartyId}/hook/ping")
                .Result(json);
        });

        // Act
        var res = await HookApi.PingPartyHooks(ExamplePartyId);

        // Assert
        Assert.AreEqual(ExamplePartyId, res);
    }

    [TestMethod]
    public async Task DeletePartyHookTest()
    {
        var targetId = ExampleHook.Id;

        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = JsonConvert.SerializeObject(ExampleHook);
            var encodedPartyId = HttpUtility.UrlEncode(ExamplePartyId);

            builder
                .Setup(HttpMethod.Delete, $"/api/v1/{encodedPartyId}/hook/{targetId}")
                .Result(json);
        });

        // Act
        await HookApi.DeletePartyHook(targetId, ExamplePartyId);

        // Assert
        // Implicitely succeeded
    }
}