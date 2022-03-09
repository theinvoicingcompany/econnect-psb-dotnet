using System.Collections.Generic;

namespace EConnect.Psb.Models;

public record InvoiceResponse(
    string Status,
    Dictionary<string, string> Reasons,
    Dictionary<string, string> Actions,
    string Note)
{
    public string Status { get; } = Status;
    public Dictionary<string, string> Reasons { get; } = Reasons;
    public Dictionary<string, string> Actions { get; } = Actions;
    public string Note { get; } = Note;
}