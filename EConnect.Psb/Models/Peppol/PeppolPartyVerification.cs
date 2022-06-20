using System;
using System.Text.Json.Serialization;

namespace EConnect.Psb.Models.Peppol;

public record PeppolPartyVerification(
    string Notes,
    DateTimeOffset VerifiedOn)
{
    public string Notes { get; } = Notes;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTimeOffset VerifiedOn { get; } = VerifiedOn;
}