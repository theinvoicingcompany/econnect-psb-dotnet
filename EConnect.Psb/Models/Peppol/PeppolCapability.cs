namespace EConnect.Psb.Models.Peppol;

public record PeppolCapability(
    string State,
    string Description)
{
    public string State { get; } = State;
    public string Description { get; } = Description;
}
