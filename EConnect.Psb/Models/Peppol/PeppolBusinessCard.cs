namespace EConnect.Psb.Models.Peppol;

public record PeppolBusinessCard(
    PeppolBusinessCardProperty Names,
    PeppolBusinessCardProperty Address,
    PeppolBusinessCardProperty EmailAddress,
    string State)
{
    public PeppolBusinessCardProperty Names { get; } = Names;
    public PeppolBusinessCardProperty Address { get; } = Address;
    public PeppolBusinessCardProperty EmailAddress { get; } = EmailAddress;
    public string State { get; } = State;
}
