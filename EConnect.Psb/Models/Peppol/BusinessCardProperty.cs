namespace EConnect.Psb.Models.Peppol;

public record BusinessCardProperty(
    string? State,
    string? Value,
    string? Description)
{
    public string? State { get; } = State;
    public string? Value { get; } = Value;
    public string? Description { get; } = Description;
}