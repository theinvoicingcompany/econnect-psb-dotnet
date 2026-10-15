using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using EConnect.Psb.Api;
using EConnect.Psb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EConnect.Psb.UnitTests.Api;

[TestClass]
public class EnrollmentApiTests : PsbTestContext
{
    public IPsbEnrollmentApi EnrollmentApi => GetRequiredService<IPsbEnrollmentApi>();

    [TestMethod]
    public async Task EnrollTest()
    {
        // Arrange
        string? body = null;

        SetAccessToken();
        Configure(builder =>
        {
            builder
                .Setup(HttpMethod.Put, "/api/v1/0106%3a12345678/enroll", message =>
                {
                    body = message.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
                    return message.Headers.Contains("X-EConnect-DomainId");
                })
                .Result(@"{ ""id"": ""enroll-doc"" }");
        });

        var enrollment = new Enrollment(
            new List<EnrollmentService>
            {
                new(EnrollmentService.PeppolService, Config: new { verification = new { notes = "Contract C21345" } })
            },
            new Dictionary<string, PartyPermissions>
            {
                { "user1", new PartyPermissions(CanSendDocument: true, CanReceiveDocument: true) }
            },
            new List<Hook>
            {
                new("1", "mailto:techsupport@econnect.eu", "mail hook", new[] { "InvoiceReceived" }, true)
            });

        // Act
        var res = await EnrollmentApi.Enroll("0106:12345678", enrollment, "domain1");

        // Assert
        Assert.AreEqual("enroll-doc", res.Id);
        Assert.IsNotNull(body);

        using var json = JsonDocument.Parse(body);
        var service = json.RootElement.GetProperty("services")[0];
        Assert.AreEqual("peppolService", service.GetProperty("name").GetString());
        Assert.IsTrue(service.GetProperty("isActive").GetBoolean());
        Assert.AreEqual("Contract C21345", service.GetProperty("config").GetProperty("verification").GetProperty("notes").GetString());
        Assert.IsTrue(json.RootElement.GetProperty("users").GetProperty("user1").GetProperty("canSendDocument").GetBoolean());
        Assert.AreEqual("mail hook", json.RootElement.GetProperty("hooks")[0].GetProperty("name").GetString());
    }

    [TestMethod]
    public async Task EnrollWithoutUsersAndHooksTest()
    {
        // Arrange
        string? body = null;

        SetAccessToken();
        Configure(builder =>
        {
            builder
                .Setup(HttpMethod.Put, "/api/v1/0106%3a12345678/enroll", message =>
                {
                    body = message.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
                    return true;
                })
                .Result(@"{ ""id"": ""enroll-doc"" }");
        });

        // Act
        await EnrollmentApi.Enroll("0106:12345678", new Enrollment(new List<EnrollmentService> { new("peppolService", false) }));

        // Assert
        using var json = JsonDocument.Parse(body!);
        Assert.IsFalse(json.RootElement.TryGetProperty("users", out _));
        Assert.IsFalse(json.RootElement.TryGetProperty("hooks", out _));
        Assert.IsFalse(json.RootElement.GetProperty("services")[0].TryGetProperty("config", out _));
        Assert.IsFalse(json.RootElement.GetProperty("services")[0].GetProperty("isActive").GetBoolean());
    }

    [TestMethod]
    public async Task UnenrollTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            builder
                .Setup(HttpMethod.Delete, "/api/v1/0106%3a12345678/enroll", ensureEConnectDomainId: true)
                .Result(@"{ ""id"": ""unenroll-doc"" }");
        });

        // Act
        var res = await EnrollmentApi.Unenroll("0106:12345678", "domain1");

        // Assert
        Assert.AreEqual("unenroll-doc", res.Id);
        VerifyDeleteRequest(Times.Once());
    }
}
