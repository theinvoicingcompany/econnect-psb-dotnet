using System;

namespace EConnect.Psb.Models;

public record DocumentValidationResult(
    bool? IsValid,
    ValidationDocumentInfo? Source,
    AssertionMessage[]? Assertions)
{
    public bool? IsValid { get; } = IsValid;
    public ValidationDocumentInfo? Source { get; } = Source;
    public AssertionMessage[] Assertions { get; } = Assertions ?? Array.Empty<AssertionMessage>();
}

public record ValidationDocumentInfo(
    string? TypeId,
    string? TypeName,
    string? ProfileId)
{
    public string? TypeId { get; } = TypeId;
    public string? TypeName { get; } = TypeName;
    public string? ProfileId { get; } = ProfileId;
}

public record AssertionMessage(
    ValidationArtifact Artifact,
    string Flag,
    string Text,
    string? Test = null,
    string? Location = null,
    string? Code = null)
{
    public ValidationArtifact Artifact { get; } = Artifact;
    public string Flag { get; } = Flag;
    public string Text { get; } = Text;
    public string? Test { get; } = Test;
    public string? Location { get; } = Location;
    public string? Code { get; } = Code;
}

public record ValidationArtifact(
    string Type,
    string Name)
{
    public string Type { get; } = Type;
    public string Name { get; } = Name;
}
