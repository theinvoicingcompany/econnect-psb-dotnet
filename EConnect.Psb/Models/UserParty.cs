using System.Collections.Generic;

namespace EConnect.Psb.Models;

public record UserParty(string Id, string Name, Dictionary<string, bool> Permissions)
{
    public string Id { get; } = Id;

    public string Name { get; } = Name;

    public Dictionary<string, bool> Permissions { get; } = Permissions;
}