namespace EConnect.Psb.Models;

public record User(string Name, string? Password)
{
    public string Name { get; } = Name;
    public string? Password { get; } = Password;
}