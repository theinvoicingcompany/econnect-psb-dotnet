namespace EConnect.Psb.Models;

public record OrderResponse(
    string Status,
    string Note)
{
    public string Status { get; } = Status;
    public string Note { get; } = Note;
}