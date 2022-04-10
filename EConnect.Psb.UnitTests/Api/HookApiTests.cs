using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using EConnect.Psb.Api;
using EConnect.Psb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EConnect.Psb.UnitTests.Api;

[TestClass]
public class HookApiTests : PsbTestContext
{
    public IPsbHookApi HookApi => GetRequiredService<IPsbHookApi>();

    [TestMethod]
    public async Task GetEnvironmentHooksTest()
    {
        // Arrange
        Hook hook = new Hook(
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
            PublishTopics: new string[] {
                "*Received"
            },
            IsActive: true
        );

        SetAccessToken();
        Configure(builder =>
        {
            var json = @"[
            {
                ""id"": ""1"",
                ""action"": ""mailto:techsupport@econnect.eu"",
                ""name"": ""mail hook"",
                ""topics"": [
                    ""*Received"",
                    ""*ReceivedError"",
                    ""*Sent"",
                    ""*SentRetry"",
                    ""*SentError"",
                    ""HookSent"",
                    ""HookSentError"",
                    ""HookSentRetry""
                ],
                ""publishTopics"": [
                    ""*Received""
                ],
                ""isActive"": true
            }]";

            builder
                .Setup(HttpMethod.Get, "/api/v1/hook")
                .Result(json);
        });

        // Act
        var res = await HookApi.GetEnvironmentHooks();

        // Assert
        Assert.IsNotNull(res);
        Assert.IsNotNull(res[0]);
        Assert.AreEqual(hook.Id, res[0].Id);
        Assert.AreEqual(hook.Action, res[0].Action);
        Assert.AreEqual(hook.Name, res[0].Name);
        for (int i = 0; i < hook.Topics.Length; i++)
        {
            Assert.AreEqual(hook.Topics[i], res[0].Topics[i]);
        }
        for (int i = 0; i < hook.PublishTopics.Length; i++)
        {
            Assert.AreEqual(hook.PublishTopics[i], res[0].PublishTopics[i]);
        }
        Assert.AreEqual(hook.IsActive, res[0].IsActive);
    }

    [TestMethod]
    public async Task PutEnvironmentHooksTest()
    {
        // Arrange
        Hook hook = new Hook(
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
            PublishTopics: new string[] {
                "*Received"
            },
            IsActive: true
        );

        SetAccessToken();
        Configure(builder =>
        {
            var json = @"{
                ""id"": ""1"",
                ""action"": ""mailto:techsupport@econnect.eu"",
                ""name"": ""mail hook"",
                ""topics"": [
                    ""*Received"",
                    ""*ReceivedError"",
                    ""*Sent"",
                    ""*SentRetry"",
                    ""*SentError"",
                    ""HookSent"",
                    ""HookSentError"",
                    ""HookSentRetry""
                ],
                ""publishTopics"": [
                    ""*Received""
                ],
                ""isActive"": true
            }";

            builder
                .Setup(HttpMethod.Put, "/api/v1/hook")
                .Result(json);
        });

        // Act
        var res = await HookApi.SetEnvironmentHook(hook);

        // Assert
        Assert.AreEqual(hook.Id, res.Id);
        Assert.AreEqual(hook.Action, res.Action);
        Assert.AreEqual(hook.Name, res.Name);
        for(int i = 0; i < hook.Topics.Length; i++)
        {
            Assert.AreEqual(hook.Topics[i], res.Topics[i]);
        }
        for (int i = 0; i < hook.PublishTopics.Length; i++)
        {
            Assert.AreEqual(hook.PublishTopics[i], res.PublishTopics[i]);
        }
        Assert.AreEqual(hook.IsActive, res.IsActive);
    }

    [TestMethod]
    public async Task DeleteDefaultHookTest()
    {
        // Arrange
        var expectedId = Guid.NewGuid().ToString();

        SetAccessToken();
        Configure(builder =>
        {
            builder
                .Setup(HttpMethod.Delete, $"/api/v1/hook/{expectedId}")
                .Result("", contentType: "plain");
        });

        // Act
        await HookApi.DeleteEnvironmentHook(expectedId);

        // Assert
        VerifyDeleteRequest(Times.Once());
    }

    [TestMethod]
    public async Task GetPartyHooksTest()
    {
        // Arrange
        string partyId = "NL:KVK:12345678";
        Hook hook = new Hook(
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
            PublishTopics: new string[] {
                "*Received"
            },
            IsActive: true
        );

        SetAccessToken();
        Configure(builder =>
        {
            var json = @"[
            {
                ""id"": ""1"",
                ""action"": ""mailto:techsupport@econnect.eu"",
                ""name"": ""mail hook"",
                ""topics"": [
                    ""*Received"",
                    ""*ReceivedError"",
                    ""*Sent"",
                    ""*SentRetry"",
                    ""*SentError"",
                    ""HookSent"",
                    ""HookSentError"",
                    ""HookSentRetry""
                ],
                ""publishTopics"": [
                    ""*Received""
                ],
                ""isActive"": true
            }]";

            var encodedPartyId = HttpUtility.UrlEncode(partyId);

            builder
                .Setup(HttpMethod.Get, $"/api/v1/{encodedPartyId}/hook")
                .Result(json);
        });

        // Act
        var res = await HookApi.GetHooks(partyId);

        // Assert
        Assert.IsNotNull(res);
        Assert.IsNotNull(res[0]);
        Assert.AreEqual(hook.Id, res[0].Id);
        Assert.AreEqual(hook.Action, res[0].Action);
        Assert.AreEqual(hook.Name, res[0].Name);
        for (int i = 0; i < hook.Topics.Length; i++)
        {
            Assert.AreEqual(hook.Topics[i], res[0].Topics[i]);
        }
        Assert.AreEqual(hook.IsActive, res[0].IsActive);
    }

    [TestMethod]
    public async Task PutPartyHooksTest()
    {
        // Arrange
        string partyId = "NL:KVK:12345678";
        Hook hook = new Hook(
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
            PublishTopics: new string[] {
                "*Received"
            },
            IsActive: true
        );

        SetAccessToken();
        Configure(builder =>
        {
            var json = @"{
                ""id"": ""1"",
                ""action"": ""mailto:techsupport@econnect.eu"",
                ""name"": ""mail hook"",
                ""topics"": [
                    ""*Received"",
                    ""*ReceivedError"",
                    ""*Sent"",
                    ""*SentRetry"",
                    ""*SentError"",
                    ""HookSent"",
                    ""HookSentError"",
                    ""HookSentRetry""
                ],
                ""publishTopics"": [
                    ""*Received""
                ],
                ""isActive"": true
            }";

            var encodedPartyId = HttpUtility.UrlEncode(partyId);

            builder
                .Setup(HttpMethod.Put, $"/api/v1/{encodedPartyId}/hook")
                .Result(json);
        });

        // Act
        var res = await HookApi.SetHook(partyId, hook);

        // Assert
        Assert.AreEqual(hook.Id, res.Id);
        Assert.AreEqual(hook.Action, res.Action);
        Assert.AreEqual(hook.Name, res.Name);
        for (int i = 0; i < hook.Topics.Length; i++)
        {
            Assert.AreEqual(hook.Topics[i], res.Topics[i]);
        }
        Assert.AreEqual(hook.IsActive, res.IsActive);
    }

    [TestMethod]
    public async Task PingPartyHooksTest()
    {
        // Arrange
        string partyId = "NL:KVK:12345678";
        Hook hook = new Hook(
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
            PublishTopics: new string[] {
                "*Received"
            },
            IsActive: true
        );

        SetAccessToken();
        Configure(builder =>
        {
            var json = "{\r\n  \"id\": \"" + partyId +"\"\r\n}";
            var encodedPartyId = HttpUtility.UrlEncode(partyId);

            builder
                .Setup(HttpMethod.Get, $"/api/v1/{encodedPartyId}/hook/ping")
                .Result(json);
        });

        // Act
        var res = await HookApi.PingHooks(partyId);

        // Assert
        Assert.AreEqual(partyId, res);
    }

    [TestMethod]
    public async Task DeletePartyHookTest()
    {
        // Arrange
        string partyId = "NL:KVK:12345678";
        var expectedId = Guid.NewGuid().ToString();

        SetAccessToken();
        Configure(builder =>
        {
            var encodedPartyId = HttpUtility.UrlEncode(partyId);

            builder
                .Setup(HttpMethod.Delete, $"/api/v1/{encodedPartyId}/hook/{expectedId}")
                .Result("", contentType: "plain");
        });

        // Act
        await HookApi.DeleteHook(partyId, expectedId);

        // Assert
        VerifyDeleteRequest(Times.Once());
    }
}