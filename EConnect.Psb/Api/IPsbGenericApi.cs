using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EConnect.Psb.Models;

namespace EConnect.Psb.Api;

public interface IPsbGenericApi
{
    Task<Document> Receive(string receiverPartyId,
        FileContent file,
        string? topic = null,
        string? senderPartyId = null,
        string? channel = null,
        bool disablePlugins = false,
        string? documentId = null,
        string? sourceFormat = null,
        string? targetFormat = null,
        IDictionary<string, string>? metaAttributes = null,
        CancellationToken cancellation = default);

    Task<Document> Send(
        string senderPartyId,
        FileContent file,
        string? topic = null,
        string? receiverPartyId = null,
        string? channel = null,
        bool disablePlugins = false,
        string? documentId = null,
        string? sourceFormat = null,
        string? targetFormat = null,
        IDictionary<string, string>? metaAttributes = null,
        CancellationToken cancellation = default);

    Task<FileContent> Download(
        string partyId,
        string documentId,
        string? targetFormat = null,
        CancellationToken cancellation = default);
}