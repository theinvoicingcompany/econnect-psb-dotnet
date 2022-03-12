using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using EConnect.Psb.Api;
using EConnect.Psb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace EConnect.Psb.UnitTests.Api;

[TestClass]
public class HookApiTests : PsbTestContext
{
    public IPsbHookApi HookApi => GetRequiredService<IPsbHookApi>();

    private readonly Hook _exampleHook = new Hook(
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
    public async Task GetEnvironmentHooksTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = $"[ {JsonConvert.SerializeObject(_exampleHook)} ]";
            builder
                .Setup(HttpMethod.Get, "/api/v1/hook")
                .Result(json);
        });

        // Act
        var res = await HookApi.GetEnvironmentHooks();

        // Assert
        Assert.IsNotNull(res);
        Assert.IsNotNull(res[0]);
        Assert.AreEqual(_exampleHook.Id, res[0].Id);
        Assert.AreEqual(_exampleHook.Action, res[0].Action);
        Assert.AreEqual(_exampleHook.Name, res[0].Name);
        for (int i = 0; i < _exampleHook.Topics.Length; i++)
        {
            Assert.AreEqual(_exampleHook.Topics[i], res[0].Topics[i]);
        }
        Assert.AreEqual(_exampleHook.IsActive, res[0].IsActive);
    }

    [TestMethod]
    public async Task PutEnvironmentHooksTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = JsonConvert.SerializeObject(_exampleHook);
            builder
                .Setup(HttpMethod.Put, "/api/v1/hook")
                .Result(json);
        });

        // Act
        var res = await HookApi.SetEnvironmentHook(_exampleHook);

        // Assert
        Assert.AreEqual(_exampleHook.Id, res.Id);
        Assert.AreEqual(_exampleHook.Action, res.Action);
        Assert.AreEqual(_exampleHook.Name, res.Name);
        for(int i = 0; i < _exampleHook.Topics.Length; i++)
        {
            Assert.AreEqual(_exampleHook.Topics[i], res.Topics[i]);
        }
        Assert.AreEqual(_exampleHook.IsActive, res.IsActive);
    }

    [TestMethod]
    public async Task DeleteDefaultHookTest()
    {
        var targetId = _exampleHook.Id;

        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = JsonConvert.SerializeObject(_exampleHook);
            builder
                .Setup(HttpMethod.Delete, $"/api/v1/hook/{targetId}")
                .Result(json);
        });

        // Act
        await HookApi.DeleteEnvironmentHook(targetId);

        // Assert
        // Implicitly succeeded
    }



    [TestMethod]
    public async Task GetPartyHooksTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = $"[ {JsonConvert.SerializeObject(_exampleHook)} ]";
            var encodedPartyId = HttpUtility.UrlEncode(ExamplePartyId);

            builder
                .Setup(HttpMethod.Get, $"/api/v1/{encodedPartyId}/hook")
                .Result(json);
        });

        // Act
        var res = await HookApi.GetHooks(ExamplePartyId);

        // Assert
        Assert.IsNotNull(res);
        Assert.IsNotNull(res[0]);
        Assert.AreEqual(_exampleHook.Id, res[0].Id);
        Assert.AreEqual(_exampleHook.Action, res[0].Action);
        Assert.AreEqual(_exampleHook.Name, res[0].Name);
        for (int i = 0; i < _exampleHook.Topics.Length; i++)
        {
            Assert.AreEqual(_exampleHook.Topics[i], res[0].Topics[i]);
        }
        Assert.AreEqual(_exampleHook.IsActive, res[0].IsActive);
    }

    [TestMethod]
    public async Task PutPartyHooksTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = JsonConvert.SerializeObject(_exampleHook);
            var encodedPartyId = HttpUtility.UrlEncode(ExamplePartyId);

            builder
                .Setup(HttpMethod.Put, $"/api/v1/{encodedPartyId}/hook")
                .Result(json);
        });

        // Act
        var res = await HookApi.SetHook(ExamplePartyId, _exampleHook);

        // Assert
        Assert.AreEqual(_exampleHook.Id, res.Id);
        Assert.AreEqual(_exampleHook.Action, res.Action);
        Assert.AreEqual(_exampleHook.Name, res.Name);
        for (int i = 0; i < _exampleHook.Topics.Length; i++)
        {
            Assert.AreEqual(_exampleHook.Topics[i], res.Topics[i]);
        }
        Assert.AreEqual(_exampleHook.IsActive, res.IsActive);
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
        var res = await HookApi.PingHooks(ExamplePartyId);

        // Assert
        Assert.AreEqual(ExamplePartyId, res);
    }

    [TestMethod]
    public async Task DeletePartyHookTest()
    {
        // Arrange
        var hookId = "hookId";
        SetAccessToken();
        Configure(builder =>
        {
            var json = JsonConvert.SerializeObject(_exampleHook);
            var encodedPartyId = HttpUtility.UrlEncode(ExamplePartyId);

            builder
                .Setup(HttpMethod.Delete, $"/api/v1/{encodedPartyId}/hook/{hookId}")
                .Result(json);
        });

        // Act
        await HookApi.DeleteHook(ExamplePartyId, hookId);

        // Assert
        VerifyDeleteRequest(Times.Once());
    }
}