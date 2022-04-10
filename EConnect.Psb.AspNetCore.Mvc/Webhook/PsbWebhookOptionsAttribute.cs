using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EConnect.Psb.AspNetCore.Mvc.Webhook;

public class PsbWebhookOptionsAttribute : PsbWebhookBaseAttribute
{
    private readonly string? _configurationName;

    public PsbWebhookOptionsAttribute(string configurationName)
    {
        _configurationName = configurationName;
    }

    public PsbWebhookOptionsAttribute(string configurationName, string template) : base(template)
    {
        _configurationName = configurationName;
    }

    protected override string? GetSecret(AuthorizationFilterContext context)
    {
        var options = context.HttpContext.RequestServices.GetRequiredService<IOptionsSnapshot<PsbWebhookOptions>>();
        if (_configurationName == null)
            return options.Value.Secret;

        var namedOptions = options.Get(_configurationName);
        return namedOptions.Secret;
    }
}