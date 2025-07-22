using System.Text.RegularExpressions;
using GeoApi.Model.DomainExceprions;

namespace GeoApi.Model;

public record Name
{
    public string Value { get; init; }

    private Name(string name)
    {
        Value = name;
    }

    public static Name CreateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new NameException("Name can't be null or empty");

        if (name.Length > 30 || name.Length < 3)
        {
            throw new NameException("Name length must be between 3 and 30 characters");
        }

        if (!Regex.IsMatch(name, @"^[a-zA-Z0-9]+$"))
        {
            throw new NameException("Name invalid");
        }

        //TODO дополнительная валидация бизнес логики по запросу.

        return new Name(name);
    }

    public override string ToString() => Value;

}
