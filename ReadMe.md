# EConnect PSB .NET Client
A reference implementation meant as an example how to use the PSB api using C#.

The Procurement Service Bus (PSB) provides a cloud managed connection to eDelivery networks like the [Peppol][0] network. 

## Requirements

- .NET Core 2.0 or higher / .NET Framework 4.6.1 or higher

## Install
- Go through the [Getting started][1] documentation.

- Install the [package][3] in your project, through nuget or another tool of choice.
```bash
Install-Package EConnect.Psb
```

### API references
For the API you can refer to the following [documentation][4].

## Setup
Make sure to initialise the service and setup the proper authentication.

### Hostbuilder
The recommended way to setup the PSB apis is by using the hostbuilder so you could get the advantages of the dependency injection framework.

```csharp
builder.Services.AddPsbService(_ =>
{
    _.Username = "{username}";
    _.Password = "{password}";
    _.PsbUrl = "{psbUrl}";
    _.IdentityUrl = "{identityUrl}";
    _.ClientId = "{clientId}";
    _.ClientSecret = "{clientSecret}";
    _.SubscriptionKey = "{SubscriptionKey}";
});
```

After the PsbService is intialized you can use the following apis:

- IPsbMeApi
- IPsbHookApi
- IPsbPeppolApi
- IPsbPurchaseInvoiceApi
- IPsbPurchaseOrderApi
- IPsbSalesInvoiceApi
- IPsbSalesOrderApi
- IPsbGenericApi

You could now for example inject the sales invoice api into your controller:

```csharp
public YourController(IPsbSalesInvoiceApi salesInvoice)
{
    _salesInvoice = salesInvoice;
}
```

All api interfaces are public which makes it possible to mock them.

### Example (web) hostbuilder

An example of the hosthuilder can be found in the project [`EConnect.Psb.Web.Example`][9]. In this project the [`.net client`][2] can be used to [`send an invoice via Peppol`][5]. \
Additionally, the supplied [`webhook receiver`][8] shows a simple implementation to receive [`invoices from Peppol`][6].


### PsbServiceHost
Use the PsbServiceHost when you are not able to use a dependency framework.

```csharp
using (var psb = PsbServiceHost.Create(_ =>
    {
    _.Username = "{username}";
    _.Password = "{password}";
    _.PsbUrl = "{psbUrl}";
    _.IdentityUrl = "{identityUrl}";
    _.ClientId = "{clientId}";
    _.ClientSecret = "{clientSecret}";
    _.SubscriptionKey = "{SubscriptionKey}";
    }
))
{
    // Example for QueryRecipientParty
    var res = await psb.SalesInvoice.QueryRecipientParty("{senderPartyId}", new[] { "{receiverPartyId}"  });
    Console.WriteLine(res);
}
```
### Example console application
An example of the PsbServiceHost can be found in the project [`EConnect.Psb.ConsoleNet.Example`][10].


## Read more

If you want to know more about Peppol e-procurement or other procurement network the go to the [`Procurement Service Bus introduction page`][7].

[0]: https://psb.econnect.eu/networks/peppol.html
[1]: https://psb.econnect.eu/introduction/gettingStarted.html
[2]: https://github.com/everbinding/econnect-psb-dotnet/blob/master/EConnect.Psb.Web.Example/Pages/ExampleSendInvoice.cshtml.cs
[3]: https://www.nuget.org/packages/EConnect.Psb/
[4]: https://psb.econnect.eu/?urls.primaryName=V1
[5]: https://psb.econnect.eu/introduction/sendInvoice.html
[6]: https://psb.econnect.eu/introduction/receiveInvoice.html
[7]: https://psb.econnect.eu/introduction/overview.html
[8]: https://github.com/everbinding/econnect-psb-dotnet/tree/master/EConnect.Psb.Web.Example/Controllers/WebhookController.cs
[9]: https://github.com/everbinding/econnect-psb-dotnet/tree/master/EConnect.Psb.Web.Example
[10]: https://github.com/everbinding/econnect-psb-dotnet/blob/master/EConnect.Psb.ConsoleNet.Example/Program.cs