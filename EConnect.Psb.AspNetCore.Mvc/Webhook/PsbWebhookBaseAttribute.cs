using System;
using System.Linq;
using System.Threading.Tasks;
using EConnect.Psb.WebHook;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EConnect.Psb.AspNetCore.Mvc.Webhook;

public abstract class PsbWebhookBaseAttribute : HttpPostAttribute, IAsyncAuthorizationFilter
{
    protected PsbWebhookBaseAttribute()
    {
    }

    protected PsbWebhookBaseAttribute(string template) : base(template)
    {
    }

    protected abstract string? GetSecret(AuthorizationFilterContext context);

    private async Task<bool> IsValidRequest(HttpRequest request, string? secret)
    {
        if (secret == null)
            return false;

        request.EnableBuffering();

        var expectedSignature = await PsbWebHookSignature.ComputeSignature(request.Body, secret).ConfigureAwait(false);

        if (!request.Headers.TryGetValue("X-EConnect-Signature", out var signatureHeaders))
            return false;

        var headerSignature = signatureHeaders.FirstOrDefault();
        return string.Equals(headerSignature, expectedSignature, StringComparison.InvariantCulture);
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var secret = GetSecret(context);

        if (!await IsValidRequest(context.HttpContext.Request, secret).ConfigureAwait(false))
            context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
    }
}