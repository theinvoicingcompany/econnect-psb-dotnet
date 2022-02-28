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
        SalesInvoice = serviceProvider.GetRequiredService<IPsbSalesInvoiceApi>();
    }

    public IPsbMeApi Me { get; }
    public IPsbHookApi Hook { get; }
    public IPsbSalesInvoiceApi SalesInvoice { get; }
}