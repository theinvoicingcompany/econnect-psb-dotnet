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
        Peppol = serviceProvider.GetRequiredService<IPsbPeppolApi>();
        PurchaseInvoice = serviceProvider.GetRequiredService<IPsbPurchaseInvoiceApi>();
        SalesInvoice = serviceProvider.GetRequiredService<IPsbSalesInvoiceApi>();
        SalesOrderApi = serviceProvider.GetRequiredService<IPsbSalesOrderApi>();
        PurchaseOrderApi = serviceProvider.GetRequiredService<IPsbPurchaseOrderApi>();
        GenericApi = serviceProvider.GetRequiredService<IPsbGenericApi>();
    }

    public IPsbMeApi Me { get; }
    public IPsbHookApi Hook { get; }
    public IPsbPeppolApi Peppol { get; }
    public IPsbPurchaseInvoiceApi PurchaseInvoice { get; }
    public IPsbSalesInvoiceApi SalesInvoice { get; }
    public IPsbGenericApi GenericApi { get; }
    public IPsbSalesOrderApi SalesOrderApi { get; }
    public IPsbPurchaseOrderApi PurchaseOrderApi { get; }
}