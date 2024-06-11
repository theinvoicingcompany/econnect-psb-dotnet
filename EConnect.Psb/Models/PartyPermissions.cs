namespace EConnect.Psb.Models;

public record PartyPermissions(
    bool CanSendDocument = false,
    bool CanReceiveDocument = false,
    bool CanRemoveDocument = false,
    bool CanManageHook = false,
    bool CanChangeDocumentStatus = false)
{
    public bool CanSendDocument { get; } = CanSendDocument;

    public bool CanReceiveDocument { get; } = CanReceiveDocument;

    public bool CanRemoveDocument { get; } = CanRemoveDocument;

    public bool CanManageHook { get; } = CanManageHook;

    public bool CanChangeDocumentStatus { get; } = CanChangeDocumentStatus;
}