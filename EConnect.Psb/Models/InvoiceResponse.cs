using System;
using System.Collections.Generic;
using System.Text;

namespace EConnect.Psb.Models;

public record InvoiceResponse(
    string Status,
    string[] Reasons,
    string[] Actions,
    string Note,
    DateTime? CreatedOn = null,
    DateTime? ChangedOn = null)
{
    public string Status { get; } = Status;
    public string[] Reasons { get; } = Reasons;
    public string[] Actions { get; } = Actions;
    public string Note { get; } = Note;
    public DateTime? CreatedOn { get; } = CreatedOn;
    public DateTime? ChangedOn { get; } = ChangedOn;
}