using System;

namespace EConnect.Psb.Models;

public record DocumentStatus(
    string Id,
    string? Name,
    string? Description,
    DateTimeOffset CreatedOn
    )
{
    public string Id { get; } = Id;
    public string? Name { get; } = Name;
    public string? Description { get; } = Description;
    public DateTimeOffset CreatedOn { get; } = CreatedOn;
}