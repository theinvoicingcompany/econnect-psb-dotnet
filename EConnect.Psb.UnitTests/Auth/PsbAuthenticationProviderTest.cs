using System.Net.Http;
using System.Threading.Tasks;
using EConnect.Psb.Auth;
using EConnect.Psb.Config;
using EConnect.Psb.Models;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Contrib.HttpClient;

namespace EConnect.Psb.UnitTests.Auth;

[TestClass]
public class PsbAuthenticationProviderTest : PsbTestContext
{
    public IPsbAuthenticationProvider Authentication => GetRequiredService<IPsbAuthenticationProvider>();

    [TestMethod]
    public async Task LoginTest()
    {
        // Arrange
        var expectedToken = "expected";
        SetAccessToken(expectedToken);

        // Act
        var token = await Authentication.GetAccessToken("ap");

        // Assert
        Assert.AreEqual(expectedToken, token);
    }

    [TestMethod]
    public async Task TokenShouldBeReusedTest()
    {
        // Arrange
        SetAccessToken();

        // Act
        var token = await Authentication.GetAccessToken("ap");
        var token2 = await Authentication.GetAccessToken("ap");

        // Assert
        Assert.AreEqual(token, token2);
        VerifyAnyRequest(Times.Once());
    }

    [TestMethod]
    public async Task TokenExpirationShouldFetchTest()
    {
        // Arrange
        SetAccessToken(token: "token1", expiresIn: -10);
            

        // Act
        var token1 = await Authentication.GetAccessToken("ap");
        SetAccessToken(token: "token2", expiresIn: 100);
        var token2 = await Authentication.GetAccessToken("ap");

        // Assert
        Assert.AreEqual("token1", token1);
        Assert.AreEqual("token2", token2);
        VerifyAnyRequest(Times.Exactly(2));
    }

    [TestMethod]
    [DataRow(null, "user", "pass", "ClientId")]
    [DataRow("", "user", "pass", "ClientId")]
    [DataRow("client_id", null, "pass", "Username")]
    [DataRow(null, null, null, "ClientId")]
    public async Task MissingOptionShouldThrowTest(string? clientId, string? username, string? password, string expectedMissingOptionName)
    {
        // Arrange
        var options = Options.Create(new PsbOptions
        {
            IdentityUrl = "https://identity.econnect.mock",
            ClientId = clientId,
            ClientSecret = "client_secret",
            Username = username,
            Password = password
        });
        var authentication = new PsbAuthenticationProvider(options, Mock.Of<IHttpClientFactory>());

        // Act
        var ex = await Assert.ThrowsExactlyAsync<EConnectException>(() => authentication.GetAccessToken("ap"));

        // Assert
        Assert.AreEqual("OAUTH03", ex.Code);
        StringAssert.Contains(ex.Message, expectedMissingOptionName);
    }

    [TestMethod]
    [DataRow("client_id", null, null, null, "ClientSecret")]
    [DataRow(null, "client_secret", null, null, "ClientId")]
    [DataRow("client_id", null, "user", "pass", "ClientSecret")]
    [DataRow("client_id", "client_secret", null, "pass", "Username")]
    public async Task MissingOptionForFlowShouldThrowTest(string? clientId, string? clientSecret, string? username, string? password, string expectedMissingOptionName)
    {
        // Arrange
        var options = Options.Create(new PsbOptions
        {
            IdentityUrl = "https://identity.econnect.mock",
            ClientId = clientId,
            ClientSecret = clientSecret,
            Username = username,
            Password = password
        });
        var authentication = new PsbAuthenticationProvider(options, Mock.Of<IHttpClientFactory>());

        // Act
        var ex = await Assert.ThrowsExactlyAsync<EConnectException>(() => authentication.GetAccessToken("ap"));

        // Assert
        Assert.AreEqual("OAUTH03", ex.Code);
        StringAssert.Contains(ex.Message, expectedMissingOptionName);
    }

    [TestMethod]
    [DataRow("user", "pass", "password")]
    [DataRow(null, null, "client_credentials")]
    [DataRow("user", null, "client_credentials")]
    public async Task FlowShouldRequestGrantTypeTest(string? username, string? password, string expectedGrantType)
    {
        // Arrange
        var options = Options.Create(new PsbOptions
        {
            IdentityUrl = "https://identity.econnect.mock",
            ClientId = "client_id",
            ClientSecret = "client_secret",
            Username = username,
            Password = password
        });

        var handler = new Mock<HttpMessageHandler>();
        handler
            .SetupRequest(HttpMethod.Post, "https://identity.econnect.mock/connect/token", message =>
                message.Content!.ReadAsStringAsync().GetAwaiter().GetResult().Contains("grant_type=" + expectedGrantType))
            .ReturnsResponse(@"{""access_token"":""token"",""expires_in"":3600,""token_type"":""Bearer""}", "application/json");

        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory
            .Setup(f => f.CreateClient(It.IsAny<string>()))
            .Returns(() => new HttpClient(handler.Object));

        var authentication = new PsbAuthenticationProvider(options, httpClientFactory.Object);

        // Act
        var token = await authentication.GetAccessToken("ap");

        // Assert
        Assert.AreEqual("token", token);
    }
}