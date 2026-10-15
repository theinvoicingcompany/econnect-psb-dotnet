using System.Text.Json.Serialization;

namespace EConnect.Psb.Models;

public record EnrollmentService(
    string Name,
    bool IsActive = true,
    object? Config = null)
{
    public const string PeppolService = "peppolService";

    public string Name { get; } = Name;
    public bool IsActive { get; } = IsActive;

    // Service specific configuration, serialized as-is. For the peppol service this is the
    // peppol config, e.g. its verification and capabilities.
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Config { get; } = Config;
}
