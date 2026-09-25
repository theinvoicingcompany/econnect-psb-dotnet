using System;

namespace EConnect.Psb.Models;

public record ValidationOption(
    string Id,
    string Name,
    string State,
    DateTimeOffset CreatedOn,
    DateTimeOffset? ChangedOn = null)
{
    public string Id { get; } = Id;
    public string Name { get; } = Name;
    public string State { get; } = State;
    public DateTimeOffset CreatedOn { get; } = CreatedOn;
    public DateTimeOffset? ChangedOn { get; } = ChangedOn;
}
