using System.Text.Json;
using EConnect.Psb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EConnect.Psb.UnitTests.WebHook
{
    [TestClass]
    public class PsbWebHookParseTests
    {
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
        public void DeserializeTest()
        {
            var @event = JsonSerializer.Deserialize<PsbWebHookEvent>(jsonWebHook, new JsonSerializerOptions(JsonSerializerDefaults.Web));

            Assert.IsNotNull(@event);
            Assert.AreEqual("InvoiceSent", @event.Topic);
            Assert.AreEqual("'Invoice' successfully sent: 'ffb4b016-5c24-4bbb-b56c-3571ec4bb6a2@econnect.eu'.", @event.Message);
            Assert.AreEqual("NL:KVK:SENDER", @event.Details["sender"]);
        }
    }
}
