using System.Collections.Generic;

namespace EConnect.Psb.Models;
public record ListResult<T>(List<T> Items)
{
    public List<T> Items { get; } = Items;
}
