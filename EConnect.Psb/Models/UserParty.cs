using System.Collections.Generic;

namespace EConnect.Psb.Models;

public record UserParty(string Id, string Name, PartyPermissions Permissions)
{
    public string Id { get; } = Id;

    public string Name { get; } = Name;

    public PartyPermissions Permissions { get; } = Permissions;
}