# Econnect Psb .net
A reference implementation meant as an example how to use the PSB api using C#.

The Procurement Service Bus (PSB) provides a cloud managed connection to eDelivery networks like the [Peppol][0] network. 

## Requirements

- npm or another tool of choice

## Install
- Go through the [Getting started][1] documentation.

- Install the [package][2] in your project, through [npm][3] or another tool of choice.
```bash
npm install everbinding/Econnect.Psb
```

#### API references
For the API you can refer to the following [documentation][4].

## Build and Test
- Make sure to initialise the service and setup the proper authentication.
```csharp
public async Task SetupHost()
{
  try
  {
    // Example how to use psb services without HostBuilder
    using (var psb = PsbServiceHost.Create(_ =>
      {
        _.Username = "{username}";
        _.Password = "{password}";
        _.PsbUrl = "{psbUrl}"; // "https://accp-psb.econnect.eu";
        _.IdentityUrl = "{identityUrl}"; // "https://accp-identity.econnect.eu";
        _.ClientId = "{clientId}";
        _.ClientSecret = "{clientSecret}";
        _.SubscriptionKey = "{SubscriptionKey}";
      }
    ))
    {
      // Example for QueryRecipientParty
      var res = await psb.SalesInvoice.QueryRecipientParty(
        "{senderPartyId}",
        new[] {
          "{receiverPartyId}"
        }
      );
      Console.WriteLine(res);
    }
  }
  catch (Exception ex)
  {
    Console.WriteLine("ERROR: " + ex.Message);
  }
}
```

- Call the MeApi, to verify the current user being registered.
```csharp
public record Me(string Name)
{
    public string Name { get; } = Name;
}

public async Task<Me> MeApiExample()
{
  Me apiResult;

  using (var meApi = psbHost.Me) {
    await MeApi.Me();
  }

  return apiResult;
}
```

## Example client

In this project, the [`.net client`][2] can be used to [`send an invoice via Peppol`][5]. \
Additionally, the supplied [`webhook receiver`][2] shows a simple implementation to receive [`invoices from Peppol`][6].

## Read more

If you want to know more about Peppol e-procurement or other procurement network the go to the [`Procurement Service Bus introduction page`][7].

[0]: https://psb.econnect.eu/networks/peppol.html
[1]: https://psb.econnect.eu/introduction/gettingStarted.html
[2]: https://github.com/everbinding/econnect-psb-dotnet
[3]: https://www.npmjs.com
[4]: https://psb.econnect.eu/?urls.primaryName=V1
[5]: https://psb.econnect.eu/introduction/sendInvoice.html
[6]: https://psb.econnect.eu/introduction/receiveInvoice.html
[7]: https://psb.econnect.eu/introduction/overview.html
