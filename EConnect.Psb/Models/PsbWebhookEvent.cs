using System;
using System.Collections.Generic;

namespace EConnect.Psb.Models
{
    public record PsbWebHookEvent(string Topic, string PartyId, string HookId, string DocumentId, string Message, Dictionary<string, string> Details, DateTimeOffset CreatedOn, DateTimeOffset SentOn)
        : PsbWebHookEventBase<Dictionary<string, string>>(Topic, PartyId, HookId, DocumentId, Message, Details, CreatedOn, SentOn)
    {
    }

    public record PsbWebHookEventBase<TDetails>(string Topic, string PartyId, string HookId, string DocumentId, string Message, TDetails Details, DateTimeOffset CreatedOn, DateTimeOffset SentOn)
    {
        public string Topic { get; } = Topic;
        public string PartyId { get; } = PartyId;
        public string HookId { get; } = HookId;
        public string DocumentId { get; } = DocumentId;
        public string Message { get; } = Message;
        public TDetails Details { get; } = Details;
        public DateTimeOffset CreatedOn { get; } = CreatedOn;
        public DateTimeOffset SentOn { get; } = SentOn;
    }
}
