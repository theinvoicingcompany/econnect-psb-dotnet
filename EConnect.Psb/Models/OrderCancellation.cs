namespace EConnect.Psb.Models;

public record OrderCancellation(
    string CancellationNote,
    string Note)
{
    public string CancellationNote { get; } = CancellationNote;
    public string Note { get; } = Note;
}