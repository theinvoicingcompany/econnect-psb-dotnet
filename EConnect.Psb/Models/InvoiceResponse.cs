using System;
using System.Collections.Generic;
using System.Text;

namespace EConnect.Psb.Models;

public record InvoiceResponse(
    string Status,
    Dictionary<string, string> Reasons,
    Dictionary<string, string> Actions,
    string Note,
    DateTime? CreatedOn = null,
    DateTime? ChangedOn = null)
{
    public string Status { get; } = Status;
    public Dictionary<string, string> Reasons { get; } = Reasons;
    public Dictionary<string, string> Actions { get; } = Actions;
    public string Note { get; } = Note;
    public DateTime? CreatedOn { get; } = CreatedOn;
    public DateTime? ChangedOn { get; } = ChangedOn;
}