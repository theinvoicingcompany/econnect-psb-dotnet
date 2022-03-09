namespace EConnect.Psb.Models.Peppol;

public record PeppolBusinessCard(
    BusinessCardProperty Names,
    BusinessCardProperty Address,
    BusinessCardProperty EmailAddress,
    string State)
{
    public BusinessCardProperty Names { get; } = Names;
    public BusinessCardProperty Address { get; } = Address;
    public BusinessCardProperty EmailAddress { get; } = EmailAddress;
    public string State { get; } = State;
}
