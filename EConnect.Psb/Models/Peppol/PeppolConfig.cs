using System;
using System.Collections.Generic;

namespace EConnect.Psb.Models.Peppol;

public record PeppolConfig(
    string Id,
    Dictionary<string, PeppolCapability> Capabilities,
    PeppolBusinessCard BusinessCard,
    PeppolPartyVerification Verification,
    DateTimeOffset? CreatedOn = null,
    DateTimeOffset? ChangedOn = null)
{
    public string Id { get; } = Id;
    public Dictionary<string, PeppolCapability> Capabilities { get; } = Capabilities;
    public PeppolBusinessCard BusinessCard { get; } = BusinessCard;
    public PeppolPartyVerification Verification { get; } = Verification;
    public DateTimeOffset? CreatedOn { get; } = CreatedOn;
    public DateTimeOffset? ChangedOn { get; } = ChangedOn;
}