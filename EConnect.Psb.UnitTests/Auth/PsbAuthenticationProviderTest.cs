using System.Threading.Tasks;
using EConnect.Psb.Auth;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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
}