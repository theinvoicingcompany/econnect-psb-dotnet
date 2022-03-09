using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using EConnect.Psb.Api;
using EConnect.Psb.Models;
using EConnect.Psb.Models.Peppol;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EConnect.Psb.UnitTests.Api;

[TestClass]
public class PeppolApiTest : PsbTestContext
{
    public IPsbPeppolApi PeppolApi => GetRequiredService<IPsbPeppolApi>();

    private readonly string ExamplePartyId = "NL:KVK:12345678";

    private DeliveryOption? ExampleDeliveryOption;

    private Dictionary<string, PeppolCapability> ExamplePeppolCapabilities = new Dictionary<string, PeppolCapability>() { };

    private PeppolPartyVerification? ExamplePeppolPartyVerification;

    private PeppolBusinessCard? ExamplePeppolBusinessCard;

    private PeppolPartyConfig? ExamplePeppolPartyConfig;

    private List<Party>? ExamplePartyPageResult = new List<Party>();

    private string ExamplePeppolPartyConfigJson = @"{ 
        ""id"":""1"",
        ""capabilities"":{
            ""invoices"":{
                ""state"":""inherited => on"",
                ""description"":""SI 1.2, SI 2.0, SI 2.0 CreditNote, BIS Billing V3, BIS Billing V3 CreditNote, BIS Billing V3 CII""
            },
            ""invoiceResponse"":{
                ""state"":""inherited => off"",
                ""description"":""PEPPOL IMR""
            },
            ""orders"":{
                ""state"":""inherited => off"",
                ""description"":""SI Order 1.2, PEPPOL Order transaction 3.0""
            }
        },
        ""businessCard"":{
            ""names"":{
                ""state"":""on"",
                ""value"":""eVerbinding, eConnect"",
                ""description"":""Business names.""
            },
            ""address"":{
                ""state"":""on"",
                ""value"":""Pelmolenlaan 16A, 3447 GW, Woerden, NL"",
                ""description"":""Geographic information.""
            },
            ""emailAddress"":{
                ""state"":""inherited => on"",
                ""value"":""techsupport@econnect.eu"",
                ""description"":""Technical contact""
            },
            ""state"":""on""
        },
        ""verification"":{
            ""notes"":""Contract agreement C21345"",
            ""verifiedOn"":""2022-02-28T16:02:52.8478308+00:00""
        },
        ""createdOn"": ""2022-02-26T21:59:43.8217536+00:00"",
        ""changedOn"": ""2022-02-26T21:59:43.8217536+00:00""
    }";

    [TestInitialize]
    public void Init()
    {
        ExampleDeliveryOption = new DeliveryOption(
            PartyId: "0106:5444158",
            DocumentTypeId: "urn:oasis:names:specification:ubl:schema:xsd:ApplicationResponse-2::ApplicationResponse##urn:www.cenbii.eu:transaction:biitrns071:ver2.0:extended:urn:www.peppol.eu:bis:peppol36a:ver1.0::2.1",
            ProcessId: "urn:www.cenbii.eu:profile:bii36:ver2.0",
            Protocol: "As4",
            Url: "https://accp-ap.econnect.eu/as4/v1",
            Certificate: "MIIF .... AT00kF4Xw=="
        );

        DateTimeOffset.TryParse("2022-02-28T16:02:52.8478308+00:00", out var verifiedOnDateTime);

        ExamplePeppolPartyVerification = new PeppolPartyVerification(
            Notes: "Contract agreement C21345",
            VerifiedOn: verifiedOnDateTime
            );

        ExamplePeppolCapabilities.Add(
            key: "invoices",
            value: new PeppolCapability(
                State: "inherited => on",
                Description: "SI 1.2, SI 2.0, SI 2.0 CreditNote, BIS Billing V3, BIS Billing V3 CreditNote, BIS Billing V3 CII"
            )
        );
        ExamplePeppolCapabilities.Add(
            key: "invoiceResponse",
            value: new PeppolCapability(
                State: "inherited => off",
                Description: "PEPPOL IMR"
            )
        );
        ExamplePeppolCapabilities.Add(
            key: "orders",
            value: new PeppolCapability(
                State: "inherited => off",
                Description: "SI Order 1.2, PEPPOL Order transaction 3.0"
            )
        );

        ExamplePeppolBusinessCard = new PeppolBusinessCard(
            Names: new BusinessCardProperty(
                Value: "eVerbinding, eConnect",
                State: "on",
                Description: "Business names."
            ),
            Address: new BusinessCardProperty(
                Value: "Pelmolenlaan 16A, 3447 GW, Woerden, NL",
                State: "on",
                Description: "Geographic information."
            ),
            EmailAddress: new BusinessCardProperty(
                Value: "techsupport@econnect.eu",
                State: "inherited => on",
                Description: "Technical contact"
            ),
            State: "on"
        );

        ExamplePeppolPartyConfig = new PeppolPartyConfig(
            Id: "1",
            Capabilities: ExamplePeppolCapabilities,
            BusinessCard: ExamplePeppolBusinessCard,
            Verification: ExamplePeppolPartyVerification,
            CreatedOn: DateTime.Parse("2022-02-26T21:59:43.8217536+00:00"),
            ChangedOn: DateTime.Parse("2022-02-26T21:59:43.8217536+00:00")
        ); ;

        ExamplePartyPageResult.Add(
            new Party(ExamplePartyId)
        );
    }

    [TestMethod]
    public async Task GetDeliveryOptionTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = @"[
                { 
                    ""partyId"": ""0106:5444158"",
                    ""documentTypeId"": ""urn:oasis:names:specification:ubl:schema:xsd:ApplicationResponse-2::ApplicationResponse##urn:www.cenbii.eu:transaction:biitrns071:ver2.0:extended:urn:www.peppol.eu:bis:peppol36a:ver1.0::2.1"",
                    ""processId"": ""urn:www.cenbii.eu:profile:bii36:ver2.0"",
                    ""protocol"": ""As4"",
                    ""url"": ""https://accp-ap.econnect.eu/as4/v1"",
                    ""certificate"": ""MIIF .... AT00kF4Xw==""
                }
            ]";

            builder
                .Setup(HttpMethod.Get, "/api/v1/peppol/deliveryOption")
                .Result(json);
        });

        // Act
        var res = await PeppolApi.GetDeliveryOption();

        // Assert
        Assert.IsNotNull(ExampleDeliveryOption);
        Assert.IsNotNull(res);
        Assert.IsNotNull(res[0]);

        Assert.AreEqual(ExampleDeliveryOption.PartyId, res[0].PartyId);
        Assert.AreEqual(ExampleDeliveryOption.DocumentTypeId, res[0].DocumentTypeId);

        Assert.AreEqual(ExampleDeliveryOption.ProcessId, res[0].ProcessId);

        Assert.AreEqual(ExampleDeliveryOption.Protocol, res[0].Protocol);
        Assert.AreEqual(ExampleDeliveryOption.Url, res[0].Url);
        Assert.AreEqual(ExampleDeliveryOption.Certificate, res[0].Certificate);
    }

    [TestMethod]
    public async Task GetEnviromentConfigTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = ExamplePeppolPartyConfigJson;

            builder
                .Setup(HttpMethod.Get, $"/api/v1/peppol/config")
                .Result(json);
        });

        // Act
        var res = await PeppolApi.GetEnviromentConfig();

        // Assert
        Assert.IsNotNull(ExamplePeppolPartyConfig);
        Assert.IsNotNull(res);

        Assert.AreEqual(ExamplePeppolPartyConfig.Id, res.Id);

        Assert.AreEqual(ExamplePeppolPartyConfig.Capabilities.Count, res.Capabilities.Count);
        Assert.AreEqual(ExamplePeppolPartyConfig.BusinessCard.Names.Value, res.BusinessCard.Names.Value);
        Assert.AreEqual(ExamplePeppolPartyConfig.BusinessCard.Address.Value, res.BusinessCard.Address.Value);
        Assert.AreEqual(ExamplePeppolPartyConfig.BusinessCard.EmailAddress.Value, res.BusinessCard.EmailAddress.Value);

        Assert.AreEqual(ExamplePeppolPartyConfig.CreatedOn, res.CreatedOn);
        Assert.AreEqual(ExamplePeppolPartyConfig.ChangedOn, res.ChangedOn);
    }

    [TestMethod]
    public async Task PutEnviromentConfigTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = ExamplePeppolPartyConfigJson;

            builder
                .Setup(HttpMethod.Put, $"/api/v1/peppol/config")
                .Result(json);
        });

        // Act
        var res = await PeppolApi.PutEnviromentConfig(config: ExamplePeppolPartyConfig);

        // Assert
        Assert.IsNotNull(ExamplePeppolPartyConfig);
        Assert.IsNotNull(res);

        Assert.AreEqual(ExamplePeppolPartyConfig.Id, res.Id);

        Assert.AreEqual(ExamplePeppolPartyConfig.Capabilities.Count, res.Capabilities.Count);
        Assert.AreEqual(ExamplePeppolPartyConfig.BusinessCard.Names.Value, res.BusinessCard.Names.Value);
        Assert.AreEqual(ExamplePeppolPartyConfig.BusinessCard.Address.Value, res.BusinessCard.Address.Value);
        Assert.AreEqual(ExamplePeppolPartyConfig.BusinessCard.EmailAddress.Value, res.BusinessCard.EmailAddress.Value);

        Assert.AreEqual(ExamplePeppolPartyConfig.CreatedOn, res.CreatedOn);
        Assert.AreEqual(ExamplePeppolPartyConfig.ChangedOn, res.ChangedOn);
    }

    [TestMethod]
    public async Task GetPartiesTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var json = @"[
                { ""Id"": ""NL:KVK:12345678"" }
            ]";

            builder
                .Setup(HttpMethod.Get, $"/api/v1/peppol/config/party")
                .Result(json);
        });

        // Act
        var res = await PeppolApi.GetParties();

        // Assert
        Assert.IsNotNull(ExamplePeppolPartyConfig);
        Assert.IsNotNull(res);

        var partyList = (List<Party>)res;



        Assert.IsNotNull(partyList[0]);
        Assert.AreEqual(ExamplePartyPageResult.Count, partyList.Count);
        for (int i = 0; i < ExamplePartyPageResult.Count; i++)
        {
            Assert.AreEqual(ExamplePartyPageResult[i], partyList[i]);
        }
    }

    [TestMethod]
    public async Task GetPartyConfigTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var encodedPartyId = HttpUtility.UrlEncode(ExamplePartyId);
            var json = ExamplePeppolPartyConfigJson;

            builder
                .Setup(HttpMethod.Get, $"/api/v1/peppol/config/party/{encodedPartyId}")
                .Result(json);
        });

        // Act
        var res = await PeppolApi.GetPartyConfig(
            partyId: ExamplePartyId
        );

        // Assert
        Assert.IsNotNull(ExamplePeppolPartyConfig);
        Assert.IsNotNull(res);

        Assert.AreEqual(ExamplePeppolPartyConfig.Id, res.Id);

        Assert.AreEqual(ExamplePeppolPartyConfig.Capabilities.Count, res.Capabilities.Count);
        Assert.AreEqual(ExamplePeppolPartyConfig.BusinessCard.Names.Value, res.BusinessCard.Names.Value);
        Assert.AreEqual(ExamplePeppolPartyConfig.BusinessCard.Address.Value, res.BusinessCard.Address.Value);
        Assert.AreEqual(ExamplePeppolPartyConfig.BusinessCard.EmailAddress.Value, res.BusinessCard.EmailAddress.Value);

        Assert.AreEqual(ExamplePeppolPartyConfig.CreatedOn, res.CreatedOn);
        Assert.AreEqual(ExamplePeppolPartyConfig.ChangedOn, res.ChangedOn);
    }

    [TestMethod]
    public async Task PutPartyConfigTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var encodedPartyId = HttpUtility.UrlEncode(ExamplePartyId);
            var json = ExamplePeppolPartyConfigJson;

            builder
                .Setup(HttpMethod.Put, $"/api/v1/peppol/config/party/{encodedPartyId}")
                .Result(json);
        });

        // Act
        var res = await PeppolApi.PutPartyConfig(
            partyId: ExamplePartyId,
            partyConfig: ExamplePeppolPartyConfig
        );

        // Assert
        Assert.IsNotNull(ExamplePeppolPartyConfig);
        Assert.IsNotNull(res);

        Assert.AreEqual(ExamplePeppolPartyConfig.Id, res.Id);

        Assert.AreEqual(ExamplePeppolPartyConfig.Capabilities.Count, res.Capabilities.Count);
        Assert.AreEqual(ExamplePeppolPartyConfig.BusinessCard.Names.Value, res.BusinessCard.Names.Value);
        Assert.AreEqual(ExamplePeppolPartyConfig.BusinessCard.Address.Value, res.BusinessCard.Address.Value);
        Assert.AreEqual(ExamplePeppolPartyConfig.BusinessCard.EmailAddress.Value, res.BusinessCard.EmailAddress.Value);

        Assert.AreEqual(ExamplePeppolPartyConfig.CreatedOn, res.CreatedOn);
        Assert.AreEqual(ExamplePeppolPartyConfig.ChangedOn, res.ChangedOn);
    }

    [TestMethod]
    public async Task DeletePartyConfigTest()
    {
        // Arrange
        SetAccessToken();
        Configure(builder =>
        {
            var encodedPartyId = HttpUtility.UrlEncode(ExamplePartyId);
            var json = ExamplePeppolPartyConfigJson;

            builder
                .Setup(HttpMethod.Delete, $"/api/v1/peppol/config/party/{encodedPartyId}")
                .Result(json);
        });

        // Act
        await PeppolApi.DeletePartyConfig(
            partyId: ExamplePartyId
        );

        // Assert
        // Implicitly succeeded
    }
}