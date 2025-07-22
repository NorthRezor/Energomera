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
            throw new NameException("Имя не может быть пустым или отсутствовать");

        if (name.Length > 30 || name.Length < 3)
        {
            throw new NameException("Длина имени от 3 до 30 сиволов");
        }

        if (!Regex.IsMatch(name, @"^[a-zA-Z0-9]+$"))
        {
            throw new NameException("Ошибка в имени");
        }

        //TODO дополнительная валидация бизнес логики по запросу.

        return new Name(name);
    }

    public override string ToString() => Value;

}
