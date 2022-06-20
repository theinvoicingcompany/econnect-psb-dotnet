using System;
using System.Text.Json.Serialization;

namespace EConnect.Psb.Models;

public record Hook(
    string Id,
    string Action,
    string Name,
    string[] Topics,
    bool IsActive,
    string[]? PublishTopics = null,
    DateTimeOffset? CreatedOn = null,
    DateTimeOffset? ChangedOn = null)
{
    public string Id { get; } = Id;
    public string Action { get; } = Action;
    public string Name { get; } = Name;
    public string[] Topics { get; } = Topics;
    public string[]? PublishTopics { get; } = PublishTopics;
    public bool IsActive { get; } = IsActive;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTimeOffset? CreatedOn { get; private set; } = CreatedOn;
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTimeOffset? ChangedOn { get; private set; } = ChangedOn;
}