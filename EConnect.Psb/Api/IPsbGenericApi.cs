using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EConnect.Psb.Models;

namespace EConnect.Psb.Api;

public interface IPsbGenericApi
{
    Task<Document> Receive(
        string receiverPartyId,
        FileContent file,
        GenericReceiveOptions? options = null,
        string? domainId = null,
        CancellationToken cancellation = default);

    // Pre-options signature, kept for binary compatibility. topic has no default so a call without
    // options/topic is not ambiguous with the overload above.
    [Obsolete("Use the Receive overload that takes GenericReceiveOptions.")]
    Task<Document> Receive(
        string receiverPartyId,
        FileContent file,
        string? topic,
        string? senderPartyId = null,
        string? channel = null,
        bool disablePlugins = false,
        string? documentId = null,
        string? sourceFormat = null,
        string? targetFormat = null,
        IDictionary<string, string>? metaAttributes = null,
        string? domainId = null,
        CancellationToken cancellation = default);

    Task<Document> Send(
        string senderPartyId,
        FileContent file,
        GenericSendOptions? options = null,
        string? domainId = null,
        CancellationToken cancellation = default);

    // Pre-options signature, kept for binary compatibility. topic has no default so a call without
    // options/topic is not ambiguous with the overload above.
    [Obsolete("Use the Send overload that takes GenericSendOptions.")]
    Task<Document> Send(
        string senderPartyId,
        FileContent file,
        string? topic,
        string? receiverPartyId = null,
        string? channel = null,
        bool disablePlugins = false,
        string? documentId = null,
        string? sourceFormat = null,
        string? targetFormat = null,
        IDictionary<string, string>? metaAttributes = null,
        string? domainId = null,
        CancellationToken cancellation = default);

    Task<FileContent> Download(
        string partyId,
        string documentId,
        string? targetFormat = null,
        string? domainId = null,
        bool disablePlugins = false,
        bool ignoreValidationErrors = false,
        CancellationToken cancellation = default);

    // Pre-disablePlugins signature, kept for binary compatibility. No defaults, so it only binds when
    // all five arguments are passed and never makes a shorter call ambiguous.
    [Obsolete("Use the Download overload that takes disablePlugins and ignoreValidationErrors.")]
    Task<FileContent> Download(
        string partyId,
        string documentId,
        string? targetFormat,
        string? domainId,
        CancellationToken cancellation);

    Task<FileContent> Transform(
        FileContent file,
        string targetFormat,
        string? sourceFormat = null,
        string? domainId = null,
        CancellationToken cancellation = default);

    Task<ValidationOption[]> GetValidationOptions(
        string? domainId = null,
        CancellationToken cancellation = default);

    Task<DocumentValidationResult> Validate(
        FileContent file,
        string? sourceFormat = null,
        string? optionId = null,
        string? languageCode = null,
        IDictionary<string, string>? metaAttributes = null,
        string? domainId = null,
        CancellationToken cancellation = default);
}
