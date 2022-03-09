using System;

namespace EConnect.Psb.Models.Peppol
{
    public record PeppolPartyVerification(
        string Notes,
        DateTimeOffset VerifiedOn)
    {
        public string Notes { get; } = Notes;
        public DateTimeOffset VerifiedOn { get; } = VerifiedOn;
    }
}
