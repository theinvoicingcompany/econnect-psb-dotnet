using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EConnect.Psb.Models;

public record Enrollment(
    IList<EnrollmentService> Services,
    IDictionary<string, PartyPermissions>? Users = null,
    IList<Hook>? Hooks = null)
{
    public IList<EnrollmentService> Services { get; } = Services;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, PartyPermissions>? Users { get; } = Users;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<Hook>? Hooks { get; } = Hooks;
}
