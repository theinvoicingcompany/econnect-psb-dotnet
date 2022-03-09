using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EConnect.Psb.Models.Peppol
{
    public record PeppolPartyConfig(
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
}

// Required due to build version mismatch?
namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit { }
}