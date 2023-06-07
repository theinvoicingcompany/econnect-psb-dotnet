using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using EConnect.Psb.Api;
using EConnect.Psb.Models;
using EConnect.Psb.Models.Peppol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EConnect.Psb.UnitTests.Api;

[TestClass]
public class PeppolApiTest : PsbTestContext
{
    public IPsbPeppolApi PeppolApi => GetRequiredService<IPsbPeppolApi>();


    [TestInitialize]
    public void Init()
    {

    }

    [TestMethod]
    public async Task GetDeliveryOptionsTest()
    {
        // Arrange
        DeliveryOption? deliveryOption = new DeliveryOption(
            PartyId: "0106:5444158",
            DocumentTypeId: "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2::Invoice##urn:cen.eu:en16931:2017#compliant#urn:fdc:peppol.eu:2017:poacc:billing:3.0::2.1",
            ProcessId: "urn:www.cenbii.eu:profile:bii36:ver2.0",
            Protocol: "As4",
            Url: "https://accp-ap.econnect.eu/as4/v1",
            Certificate: "MIIF .... AT00kF4Xw=="
        );
        SetAccessToken();

        Configure(builder =>
        {
            var json = @"[
                { 
                    ""partyId"": ""0106:5444158"",
                    ""documentTypeId"": ""urn:oasis:names:specification:ubl:schema:xsd:Invoice-2::Invoice##urn:cen.eu:en16931:2017#compliant#urn:fdc:peppol.eu:2017:poacc:billing:3.0::2.1"",
                    ""processId"": ""urn:www.cenbii.eu:profile:bii36:ver2.0"",
                    ""protocol"": ""As4"",
                    ""url"": ""https://accp-ap.econnect.eu/as4/v1"",
                    ""certificate"": ""MIIF .... AT00kF4Xw==""
                }
            ]";

            builder
                .Setup(HttpMethod.Get, "/api/v1/peppol/deliveryOption?partyIds=0106%3A5444158&partyIds=9944%3ANL851306469B01&preferredDocumentTypeId=urn%3Aoasis%3Anames%3Aspecification%3Aubl%3Aschema%3Axsd%3AInvoice-2%3A%3AInvoice%23%23urn%3Acen.eu%3Aen16931%3A2017%23compliant%23urn%3Afdc%3Apeppol.eu%3A2017%3Apoacc%3Abilling%3A3.0%3A%3A2.1&documentTypeIds=urn%3Aoasis%3Anames%3Aspecification%3Aubl%3Aschema%3Axsd%3AInvoice-2%3A%3AInvoice%23%23urn%3Acen.eu%3Aen16931%3A2017%23compliant%23urn%3Afdc%3Apeppol.eu%3A2017%3Apoacc%3Abilling%3A3.0%3A%3A2.1&documentTypeIds=urn%3Aoasis%3Anames%3Aspecification%3Aubl%3Aschema%3Axsd%3AInvoice-2%3A%3AInvoice%23%23urn%3Acen.eu%3Aen16931%3A2017%23compliant%23urn%3Afdc%3Anen.nl%3Anlcius%3Av1.0%3A%3A2.1&documentFamily=Invoice&isCredit=False")
                .Result(json);
        });

        // Act
        var res = await PeppolApi.GetDeliveryOptions(
            new List<string>(){ "0106:5444158", "9944:NL851306469B01"},
            "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2::Invoice##urn:cen.eu:en16931:2017#compliant#urn:fdc:peppol.eu:2017:poacc:billing:3.0::2.1",
            new List<string>()
            {
                "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2::Invoice##urn:cen.eu:en16931:2017#compliant#urn:fdc:peppol.eu:2017:poacc:billing:3.0::2.1",
                "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2::Invoice##urn:cen.eu:en16931:2017#compliant#urn:fdc:nen.nl:nlcius:v1.0::2.1"
            },
            "Invoice",
            false);

        // Assert
        Assert.IsNotNull(deliveryOption);
        Assert.IsNotNull(res);
        Assert.IsNotNull(res[0]);

        Assert.AreEqual(deliveryOption.PartyId, res[0].PartyId);
        Assert.AreEqual(deliveryOption.DocumentTypeId, res[0].DocumentTypeId);

        Assert.AreEqual(deliveryOption.ProcessId, res[0].ProcessId);

        Assert.AreEqual(deliveryOption.Protocol, res[0].Protocol);
        Assert.AreEqual(deliveryOption.Url, res[0].Url);
        Assert.AreEqual(deliveryOption.Certificate, res[0].Certificate);
    }

    [TestMethod]
    public async Task GetEnviromentConfigTest()
    {
        // Arrange
        Dictionary<string, PeppolCapability> peppolCapabilities = new Dictionary<string, PeppolCapability>() {
            {
                "invoices",
                new PeppolCapability(
                    State: "inherited => on",
                    Description: "SI 1.2, SI 2.0, SI 2.0 CreditNote, BIS Billing V3, BIS Billing V3 CreditNote, BIS Billing V3 CII"
                )
            },
            {
                "invoiceResponse",
                new PeppolCapability(
                    State: "inherited => off",
                    Description: "PEPPOL IMR"
                )
            },
            {
                "orders",
                new PeppolCapability(
                    State: "inherited => off",
                    Description: "SI Order 1.2, PEPPOL Order transaction 3.0"
                )
            }
        };

        PeppolBusinessCard peppolBusinessCard = new PeppolBusinessCard(
            Names: new PeppolBusinessCardProperty(
                Value: "eVerbinding, eConnect",
                State: "on",
                Description: "Business names."
            ),
            Address: new PeppolBusinessCardProperty(
                Value: "Pelmolenlaan 16A, 3447 GW, Woerden, NL",
                State: "on",
                Description: "Geographic information."
            ),
            EmailAddress: new PeppolBusinessCardProperty(
                Value: "techsupport@econnect.eu",
                State: "inherited => on",
                Description: "Technical contact"
            ),
            State: "on"
        );

        DateTimeOffset.TryParse("2022-02-28T16:02:52.8478308+00:00", out var verifiedOnDateTime);
        PeppolPartyVerification? peppolPartyVerification = new PeppolPartyVerification(
             Notes: "Contract agreement C21345",
             VerifiedOn: verifiedOnDateTime
         );


        PeppolConfig peppolPartyConfig = new PeppolConfig(
            Id: "1",
            Capabilities: peppolCapabilities,
            BusinessCard: peppolBusinessCard,
            Verification: peppolPartyVerification,
            CreatedOn: DateTime.Parse("2022-02-26T21:59:43.8217536+00:00"),
            ChangedOn: DateTime.Parse("2022-02-26T21:59:43.8217536+00:00")
        );

        SetAccessToken();
        Configure(builder =>
        {
            var json = @"{ 
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

            builder
                .Setup(HttpMethod.Get, $"/api/v1/peppol/config")
                .Result(json);
        });

        // Act
        var res = await PeppolApi.GetEnvironmentConfig();

        // Assert
        Assert.IsNotNull(peppolPartyConfig);
        Assert.IsNotNull(res);

        Assert.AreEqual(peppolPartyConfig.Id, res.Id);

        Assert.AreEqual(peppolPartyConfig.Capabilities.Count, res.Capabilities.Count);
        Assert.AreEqual(peppolPartyConfig.BusinessCard.Names.Value, res.BusinessCard.Names.Value);
        Assert.AreEqual(peppolPartyConfig.BusinessCard.Address.Value, res.BusinessCard.Address.Value);
        Assert.AreEqual(peppolPartyConfig.BusinessCard.EmailAddress.Value, res.BusinessCard.EmailAddress.Value);

        Assert.AreEqual(peppolPartyConfig.CreatedOn, res.CreatedOn);
        Assert.AreEqual(peppolPartyConfig.ChangedOn, res.ChangedOn);
    }

    [TestMethod]
    public async Task PutEnviromentConfigTest()
    {
        // Arrange
        Dictionary<string, PeppolCapability> peppolCapabilities = new Dictionary<string, PeppolCapability>() {
            {
                "invoices",
                new PeppolCapability(
                    State: "inherited => on",
                    Description: "SI 1.2, SI 2.0, SI 2.0 CreditNote, BIS Billing V3, BIS Billing V3 CreditNote, BIS Billing V3 CII"
                )
            },
            {
                "invoiceResponse",
                new PeppolCapability(
                    State: "inherited => off",
                    Description: "PEPPOL IMR"
                )
            },
            {
                "orders",
                new PeppolCapability(
                    State: "inherited => off",
                    Description: "SI Order 1.2, PEPPOL Order transaction 3.0"
                )
            }
        };

        PeppolBusinessCard peppolBusinessCard = new PeppolBusinessCard(
            Names: new PeppolBusinessCardProperty(
                Value: "eVerbinding, eConnect",
                State: "on",
                Description: "Business names."
            ),
            Address: new PeppolBusinessCardProperty(
                Value: "Pelmolenlaan 16A, 3447 GW, Woerden, NL",
                State: "on",
                Description: "Geographic information."
            ),
            EmailAddress: new PeppolBusinessCardProperty(
                Value: "techsupport@econnect.eu",
                State: "inherited => on",
                Description: "Technical contact"
            ),
            State: "on"
        );

        DateTimeOffset.TryParse("2022-02-28T16:02:52.8478308+00:00", out var verifiedOnDateTime);
        PeppolPartyVerification? peppolPartyVerification = new PeppolPartyVerification(
             Notes: "Contract agreement C21345",
             VerifiedOn: verifiedOnDateTime
         );


        PeppolConfig peppolPartyConfig = new PeppolConfig(
            Id: "1",
            Capabilities: peppolCapabilities,
            BusinessCard: peppolBusinessCard,
            Verification: peppolPartyVerification,
            CreatedOn: DateTime.Parse("2022-02-26T21:59:43.8217536+00:00"),
            ChangedOn: DateTime.Parse("2022-02-26T21:59:43.8217536+00:00")
        );

        SetAccessToken();
        Configure(builder =>
        {
            var json = @"{ 
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

            builder
                .Setup(HttpMethod.Put, $"/api/v1/peppol/config")
                .Result(json);
        });

        // Act
        var res = await PeppolApi.PutEnvironmentConfig(config: peppolPartyConfig);

        // Assert
        Assert.IsNotNull(peppolPartyConfig);
        Assert.IsNotNull(res);

        Assert.AreEqual(peppolPartyConfig.Id, res.Id);

        Assert.AreEqual(peppolPartyConfig.Capabilities.Count, res.Capabilities.Count);
        Assert.AreEqual(peppolPartyConfig.BusinessCard.Names.Value, res.BusinessCard.Names.Value);
        Assert.AreEqual(peppolPartyConfig.BusinessCard.Address.Value, res.BusinessCard.Address.Value);
        Assert.AreEqual(peppolPartyConfig.BusinessCard.EmailAddress.Value, res.BusinessCard.EmailAddress.Value);

        Assert.AreEqual(peppolPartyConfig.CreatedOn, res.CreatedOn);
        Assert.AreEqual(peppolPartyConfig.ChangedOn, res.ChangedOn);
    }

    [TestMethod]
    public async Task GetPartiesTest()
    {
        // Arrange
        string partyId = "NL:KVK:12345678";

        Dictionary<string, PeppolCapability> peppolCapabilities = new Dictionary<string, PeppolCapability>() {
            {
                "invoices",
                new PeppolCapability(
                    State: "inherited => on",
                    Description: "SI 1.2, SI 2.0, SI 2.0 CreditNote, BIS Billing V3, BIS Billing V3 CreditNote, BIS Billing V3 CII"
                )
            },
            {
                "invoiceResponse",
                new PeppolCapability(
                    State: "inherited => off",
                    Description: "PEPPOL IMR"
                )
            },
            {
                "orders",
                new PeppolCapability(
                    State: "inherited => off",
                    Description: "SI Order 1.2, PEPPOL Order transaction 3.0"
                )
            }
        };

        PeppolBusinessCard peppolBusinessCard = new PeppolBusinessCard(
            Names: new PeppolBusinessCardProperty(
                Value: "eVerbinding, eConnect",
                State: "on",
                Description: "Business names."
            ),
            Address: new PeppolBusinessCardProperty(
                Value: "Pelmolenlaan 16A, 3447 GW, Woerden, NL",
                State: "on",
                Description: "Geographic information."
            ),
            EmailAddress: new PeppolBusinessCardProperty(
                Value: "techsupport@econnect.eu",
                State: "inherited => on",
                Description: "Technical contact"
            ),
            State: "on"
        );

        DateTimeOffset.TryParse("2022-02-28T16:02:52.8478308+00:00", out var verifiedOnDateTime);
        PeppolPartyVerification? peppolPartyVerification = new PeppolPartyVerification(
             Notes: "Contract agreement C21345",
             VerifiedOn: verifiedOnDateTime
         );

        PeppolConfig peppolPartyConfig = new PeppolConfig(
            Id: "1",
            Capabilities: peppolCapabilities,
            BusinessCard: peppolBusinessCard,
            Verification: peppolPartyVerification,
            CreatedOn: DateTime.Parse("2022-02-26T21:59:43.8217536+00:00"),
            ChangedOn: DateTime.Parse("2022-02-26T21:59:43.8217536+00:00")
        );

        Party[] partyPageResult = new Party[] {
            new Party(partyId)
        };

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
        Assert.IsNotNull(peppolPartyConfig);
        Assert.IsNotNull(res);

        var partyArray = res;

        Assert.IsNotNull(partyArray[0]);
        Assert.AreEqual(partyPageResult.Length, partyArray.Length);
        for (int i = 0; i < partyPageResult.Length; i++)
        {
            Assert.AreEqual(partyPageResult[i], partyArray[i]);
        }
    }

    [TestMethod]
    public async Task GetPartyConfigTest()
    {
        // Arrange
        string partyId = "NL:KVK:12345678";

        Dictionary<string, PeppolCapability> peppolCapabilities = new Dictionary<string, PeppolCapability>() {
            {
                "invoices",
                new PeppolCapability(
                    State: "inherited => on",
                    Description: "SI 1.2, SI 2.0, SI 2.0 CreditNote, BIS Billing V3, BIS Billing V3 CreditNote, BIS Billing V3 CII"
                )
            },
            {
                "invoiceResponse",
                new PeppolCapability(
                    State: "inherited => off",
                    Description: "PEPPOL IMR"
                )
            },
            {
                "orders",
                new PeppolCapability(
                    State: "inherited => off",
                    Description: "SI Order 1.2, PEPPOL Order transaction 3.0"
                )
            }
        };

        PeppolBusinessCard peppolBusinessCard = new PeppolBusinessCard(
            Names: new PeppolBusinessCardProperty(
                Value: "eVerbinding, eConnect",
                State: "on",
                Description: "Business names."
            ),
            Address: new PeppolBusinessCardProperty(
                Value: "Pelmolenlaan 16A, 3447 GW, Woerden, NL",
                State: "on",
                Description: "Geographic information."
            ),
            EmailAddress: new PeppolBusinessCardProperty(
                Value: "techsupport@econnect.eu",
                State: "inherited => on",
                Description: "Technical contact"
            ),
            State: "on"
        );

        DateTimeOffset.TryParse("2022-02-28T16:02:52.8478308+00:00", out var verifiedOnDateTime);
        PeppolPartyVerification? peppolPartyVerification = new PeppolPartyVerification(
             Notes: "Contract agreement C21345",
             VerifiedOn: verifiedOnDateTime
         );


        PeppolConfig peppolPartyConfig = new PeppolConfig(
            Id: "1",
            Capabilities: peppolCapabilities,
            BusinessCard: peppolBusinessCard,
            Verification: peppolPartyVerification,
            CreatedOn: DateTime.Parse("2022-02-26T21:59:43.8217536+00:00"),
            ChangedOn: DateTime.Parse("2022-02-26T21:59:43.8217536+00:00")
        );

        SetAccessToken();
        Configure(builder =>
        {
            var encodedPartyId = HttpUtility.UrlEncode(partyId);
            var json = @"{ 
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

            builder
                .Setup(HttpMethod.Get, $"/api/v1/peppol/config/party/{encodedPartyId}")
                .Result(json);
        });

        // Act
        var res = await PeppolApi.GetConfig(
            partyId: partyId
        );

        // Assert
        Assert.IsNotNull(peppolPartyConfig);
        Assert.IsNotNull(res);

        Assert.AreEqual(peppolPartyConfig.Id, res.Id);

        Assert.AreEqual(peppolPartyConfig.Capabilities.Count, res.Capabilities.Count);
        Assert.AreEqual(peppolPartyConfig.BusinessCard.Names.Value, res.BusinessCard.Names.Value);
        Assert.AreEqual(peppolPartyConfig.BusinessCard.Address.Value, res.BusinessCard.Address.Value);
        Assert.AreEqual(peppolPartyConfig.BusinessCard.EmailAddress.Value, res.BusinessCard.EmailAddress.Value);

        Assert.AreEqual(peppolPartyConfig.CreatedOn, res.CreatedOn);
        Assert.AreEqual(peppolPartyConfig.ChangedOn, res.ChangedOn);
    }

    [TestMethod]
    public async Task PutPartyConfigTest()
    {
        // Arrange
        string partyId = "NL:KVK:12345678";

        Dictionary<string, PeppolCapability> peppolCapabilities = new Dictionary<string, PeppolCapability>() {
            {
                "invoices",
                new PeppolCapability(
                    State: "inherited => on",
                    Description: "SI 1.2, SI 2.0, SI 2.0 CreditNote, BIS Billing V3, BIS Billing V3 CreditNote, BIS Billing V3 CII"
                )
            },
            {
                "invoiceResponse",
                new PeppolCapability(
                    State: "inherited => off",
                    Description: "PEPPOL IMR"
                )
            },
            {
                "orders",
                new PeppolCapability(
                    State: "inherited => off",
                    Description: "SI Order 1.2, PEPPOL Order transaction 3.0"
                )
            }
        };

        PeppolBusinessCard peppolBusinessCard = new PeppolBusinessCard(
            Names: new PeppolBusinessCardProperty(
                Value: "eVerbinding, eConnect",
                State: "on",
                Description: "Business names."
            ),
            Address: new PeppolBusinessCardProperty(
                Value: "Pelmolenlaan 16A, 3447 GW, Woerden, NL",
                State: "on",
                Description: "Geographic information."
            ),
            EmailAddress: new PeppolBusinessCardProperty(
                Value: "techsupport@econnect.eu",
                State: "inherited => on",
                Description: "Technical contact"
            ),
            State: "on"
        );

        DateTimeOffset.TryParse("2022-02-28T16:02:52.8478308+00:00", out var verifiedOnDateTime);
        PeppolPartyVerification? peppolPartyVerification = new PeppolPartyVerification(
             Notes: "Contract agreement C21345",
             VerifiedOn: verifiedOnDateTime
         );

        PeppolConfig peppolPartyConfig = new PeppolConfig(
            Id: "1",
            Capabilities: peppolCapabilities,
            BusinessCard: peppolBusinessCard,
            Verification: peppolPartyVerification,
            CreatedOn: DateTime.Parse("2022-02-26T21:59:43.8217536+00:00"),
            ChangedOn: DateTime.Parse("2022-02-26T21:59:43.8217536+00:00")
        );

        SetAccessToken();
        Configure(builder =>
        {
            var encodedPartyId = HttpUtility.UrlEncode(partyId);
            var json = @"{ 
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

            builder
                .Setup(HttpMethod.Put, $"/api/v1/peppol/config/party/{encodedPartyId}")
                .Result(json);
        });

        // Act
        var res = await PeppolApi.PutConfig(
            partyId: partyId,
            config: peppolPartyConfig
        );

        // Assert
        Assert.IsNotNull(peppolPartyConfig);
        Assert.IsNotNull(res);

        Assert.AreEqual(peppolPartyConfig.Id, res.Id);

        Assert.AreEqual(peppolPartyConfig.Capabilities.Count, res.Capabilities.Count);
        Assert.AreEqual(peppolPartyConfig.BusinessCard.Names.Value, res.BusinessCard.Names.Value);
        Assert.AreEqual(peppolPartyConfig.BusinessCard.Address.Value, res.BusinessCard.Address.Value);
        Assert.AreEqual(peppolPartyConfig.BusinessCard.EmailAddress.Value, res.BusinessCard.EmailAddress.Value);

        Assert.AreEqual(peppolPartyConfig.CreatedOn, res.CreatedOn);
        Assert.AreEqual(peppolPartyConfig.ChangedOn, res.ChangedOn);
    }

    [TestMethod]
    public async Task DeletePartyConfigTest()
    {
        // Arrange
        string partyId = "NL:KVK:12345678";

        SetAccessToken();
        Configure(builder =>
        {
            var encodedPartyId = HttpUtility.UrlEncode(partyId);

            builder
                .Setup(HttpMethod.Delete, $"/api/v1/peppol/config/party/{encodedPartyId}")
                .Result("", contentType: "plain");
        });

        // Act
        await PeppolApi.DeleteConfig(
            partyId: partyId
        );

        // Assert
        VerifyDeleteRequest(Times.Once());
    }
}