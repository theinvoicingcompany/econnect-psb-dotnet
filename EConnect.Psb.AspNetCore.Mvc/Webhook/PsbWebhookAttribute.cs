using Microsoft.AspNetCore.Mvc.Filters;

namespace EConnect.Psb.AspNetCore.Mvc.Webhook;

public class PsbWebhookAttribute : PsbWebhookBaseAttribute
{
    private readonly string _secret;

    public PsbWebhookAttribute(string secret)
    {
        _secret = secret;
    }

    public PsbWebhookAttribute(string secret, string template) : base(template)
    {
        _secret = secret;
    }

    protected override string? GetSecret(AuthorizationFilterContext context)
    {
        return _secret;
    }
}