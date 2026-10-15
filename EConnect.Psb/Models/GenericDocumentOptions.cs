using System.Collections.Generic;

namespace EConnect.Psb.Models;

public abstract class GenericDocumentOptions
{
    public string? Topic { get; set; }
    public string? Channel { get; set; }
    public bool DisablePlugins { get; set; }
    public string? DocumentId { get; set; }
    public string? SourceFormat { get; set; }
    public string? TargetFormat { get; set; }
    public string? Flow { get; set; }
    public IDictionary<string, string>? MetaAttributes { get; set; }
}

public class GenericSendOptions : GenericDocumentOptions
{
    public string? ReceiverPartyId { get; set; }
}

public class GenericReceiveOptions : GenericDocumentOptions
{
    public string? SenderPartyId { get; set; }
}
