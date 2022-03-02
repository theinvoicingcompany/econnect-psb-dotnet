using System.IO;
using System.Text;
using System.Threading.Tasks;
using EConnect.Psb.WebHook;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EConnect.Psb.UnitTests.WebHook
{
    [TestClass]
    public class PsbWebHookSignatureTests
    {
        private const string secret = "secret";
        private const string signature = "sha256=8f3382eadc9a5444977205979f22d5a899cae74b75b6aade80c3274b214096f4";
        private const string jsonWebHook = @"{
    ""topic"": ""InvoiceSent"",
    ""partyId"": ""NL:KVK:SENDER"",
    ""hookId"": ""111"",
    ""documentId"": ""f96bad15-0500-4d80-b883-1a82c01b8f93"",
    ""message"": ""'Invoice' successfully sent: 'ffb4b016-5c24-4bbb-b56c-3571ec4bb6a2@econnect.eu'."",
    ""details"": {
        ""sender"": ""NL:KVK:SENDER"",
        ""id"": ""12115120"",
        ""sourceDocumentTypeId"": ""urn:oasis:names:specification:ubl:schema:xsd:Invoice-2::Invoice##urn:cen.eu:en16931:2017#compliant#urn:fdc:nen.nl:nlcius:v1.0::2.1"",
        ""attempt"": ""1"",
        ""targetDocumentTypeId"": ""urn:oasis:names:specification:ubl:schema:xsd:Invoice-2::Invoice##urn:cen.eu:en16931:2017#compliant#urn:fdc:nen.nl:nlcius:v1.0::2.1"",
        ""endpoint"": ""https://accp-ap.econnect.eu/as4/v1"",
        ""recipient"": ""NL:KVK:RECEIVER"",
        ""protocol"": ""As4"",
        ""messageId"": ""f96bad15-0500-4d80-b883-1a82c01b8f93@econnect.eu"",
        ""returnedMessageId"": ""ffb4b016-5c24-4bbb-b56c-3571ec4bb6a2@econnect.eu""
    },
    ""createdOn"": ""2022-01-21T10:03:18.3726197+00:00"",
    ""sentOn"": ""2022-01-21T10:03:18.5951881+00:00""
}";

        [TestMethod]
        public void ComputeSignatureFromStringTest()
        {
            // Act
            var computeSignature = PsbWebHookSignature.ComputeSignature(jsonWebHook, secret);

            // Assert
            Assert.AreEqual(signature, computeSignature);
        }

        [TestMethod]
        public async Task ComputeSignatureFromStreamTest()
        {
            // Arrange
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonWebHook));

            // Act
            var computeSignature = await PsbWebHookSignature.ComputeSignature(stream, secret);

            // Assert
            Assert.AreEqual(signature, computeSignature);
        }
    }
}
