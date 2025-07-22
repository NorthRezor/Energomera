using System.Collections.Frozen;
using GeoApi.Model;

namespace GeoApi.Infrastructure;

public class DataStorageService
{
    public readonly FrozenDictionary<int, Field> Fields;

    public DataStorageService(ICollection<Field> fields)
    {
        Fields = fields.ToFrozenDictionary(f => f.Id, f => f)
            ?? FrozenDictionary<int, Field>.Empty;
    }
}
