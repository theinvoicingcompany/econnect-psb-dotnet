using System;
using System.ComponentModel.DataAnnotations;
using EConnect.Psb.Models;

namespace EConnect.Psb.AspNetCore.Mvc.Webhook;

public class PsbWebhookValidSentOnAttribute : ValidationAttribute
{
    private static bool WithinTimeLimit(DateTimeOffset sentOn)
    {
        var now = DateTimeOffset.UtcNow;
        var span = TimeSpan.FromMinutes(5);

        var minTimeOffset = now.Subtract(span);
        var maxTimeOffset = now.Add(span);

        return (
            sentOn > minTimeOffset &&
            sentOn < maxTimeOffset
        );
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is PsbWebHookEvent webhook && WithinTimeLimit(webhook.SentOn))
        {
            return ValidationResult.Success;
        }
        return new ValidationResult("PsbWebhookEvent does not fall in the expected time range. The event is too old or too new.");
    }
}