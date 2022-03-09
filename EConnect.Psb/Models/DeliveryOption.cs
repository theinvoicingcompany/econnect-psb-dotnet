namespace EConnect.Psb.Models;

public record DeliveryOption(
    string PartyId,
    string DocumentTypeId,
    string ProcessId,
    string Protocol,
    string Url,
    string Certificate)
{
    public string PartyId { get; } = PartyId;
    public string DocumentTypeId { get; } = DocumentTypeId;
    public string ProcessId { get; } = ProcessId;
    public string Protocol { get; } = Protocol;
    public string Url { get; } = Url;
    public string Certificate { get; } = Certificate;
}