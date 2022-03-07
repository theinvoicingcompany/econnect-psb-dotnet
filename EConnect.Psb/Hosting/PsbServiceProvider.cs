using System;
using EConnect.Psb.Api;
using Microsoft.Extensions.DependencyInjection;

namespace EConnect.Psb.Hosting;

public class PsbServiceProvider
{
    public PsbServiceProvider(IServiceProvider serviceProvider)
    {
        Me = serviceProvider.GetRequiredService<IPsbMeApi>();
        Hook = serviceProvider.GetRequiredService<IPsbHookApi>();
        PurchaseInvoice = serviceProvider.GetRequiredService<IPsbPurchaseInvoiceApi>();
        SalesInvoice = serviceProvider.GetRequiredService<IPsbSalesInvoiceApi>();
        SalesOrderApi = serviceProvider.GetRequiredService<IPsbSalesOrderApi>();
    }

    public IPsbMeApi Me { get; }
    public IPsbHookApi Hook { get; }
    public IPsbPurchaseInvoiceApi PurchaseInvoice { get; }
    public IPsbSalesInvoiceApi SalesInvoice { get; }
    public IPsbSalesOrderApi SalesOrderApi { get; }
}