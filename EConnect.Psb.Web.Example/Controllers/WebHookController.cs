using EConnect.Psb.Api;
using EConnect.Psb.Models;
using Microsoft.AspNetCore.Mvc;
using EConnect.Psb.AspNetCore.Mvc.Webhook;

namespace EConnect.Psb.Web.Example.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : Controller
{
    private readonly IPsbPurchaseInvoiceApi _purchaseInvoiceApi;
    private readonly IPsbHookApi _hookApi;

    public WebhookController(IPsbPurchaseInvoiceApi purchaseInvoiceApi, IPsbMeApi meApi, IPsbHookApi hookApi)
    {
        _purchaseInvoiceApi = purchaseInvoiceApi;
        _hookApi = hookApi;
    }

    // Hardcoded secret
    [PsbWebhook("mySecret")]
    public async Task<IActionResult> InvoiceReceived([PsbWebhookValidSentOn] PsbWebHookEvent @event)
    {
        var file = await _purchaseInvoiceApi.Download(@event.PartyId, @event.DocumentId, cancellation: HttpContext.RequestAborted).ConfigureAwait(false);
        // Process xml here
        var xml = await file.Content.ReadAsStringAsync().ConfigureAwait(false);

        return Ok();
    }

    // Secret from config
    [PsbWebhookOptions("webhook")]
    public IActionResult InvoiceSent([PsbWebhookValidSentOn] PsbWebHookEvent @event)
    {
        // update status

        return Ok();
    }
}