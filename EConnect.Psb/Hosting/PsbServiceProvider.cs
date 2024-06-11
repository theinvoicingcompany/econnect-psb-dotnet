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
        SalesOrder = serviceProvider.GetRequiredService<IPsbSalesOrderApi>();
        PurchaseOrder = serviceProvider.GetRequiredService<IPsbPurchaseOrderApi>();
        Generic = serviceProvider.GetRequiredService<IPsbGenericApi>();
        User = serviceProvider.GetRequiredService<IPsbUserApi>();
    }

    public IPsbMeApi Me { get; }
    public IPsbHookApi Hook { get; }
    public IPsbPeppolApi Peppol { get; }
    public IPsbPurchaseInvoiceApi PurchaseInvoice { get; }
    public IPsbSalesInvoiceApi SalesInvoice { get; }
    public IPsbGenericApi Generic { get; }
    public IPsbSalesOrderApi SalesOrder { get; }
    public IPsbPurchaseOrderApi PurchaseOrder { get; }
    public IPsbUserApi User { get; }
}