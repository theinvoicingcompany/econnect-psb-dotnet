namespace EConnect.Psb.Models.Peppol;

public record PrepareToMigrateResponse(string? MigrationKey)
{
    public string? MigrationKey { get; } = MigrationKey;
}
