using System.Text.Json.Serialization;

namespace EConnect.Psb.Models;

// Explicit constructors instead of a positional record: the single-argument constructor and
// deconstructor of the original Party(string Id) are kept for binary compatibility, and a second
// public constructor needs [JsonConstructor] so System.Text.Json still knows which one to use.
// Parameter names stay PascalCase to match the original record's named arguments.
public record Party
{
    [JsonConstructor]
    public Party(string Id, string? Channel = null, string? Description = null)
    {
        this.Id = Id;
        this.Channel = Channel;
        this.Description = Description;
    }

    public Party(string Id) : this(Id, null, null)
    {
    }

    public string Id { get; }
    public string? Channel { get; }
    public string? Description { get; }

    public void Deconstruct(out string Id) => Id = this.Id;

    public void Deconstruct(out string Id, out string? Channel, out string? Description)
    {
        Id = this.Id;
        Channel = this.Channel;
        Description = this.Description;
    }
}
