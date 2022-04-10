using System;
using System.Collections.Generic;
using EConnect.Psb.Models;

namespace EConnect.Psb.AspNetCore.Mvc.UnitTests.Helpers;

internal static class PsbWebHookExamples
{
    internal static PsbWebHookEvent InvoiceSent(DateTimeOffset sentOn)
    {
        return new PsbWebHookEvent("InvoiceSent", "GLN:123456789", "hook1",
            "doc1", "msg",
            new Dictionary<string, string>() { { "id", "id1" } }, DateTimeOffset.UtcNow, sentOn);
    }
}