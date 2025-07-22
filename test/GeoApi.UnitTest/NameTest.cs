using GeoApi.Model;
using GeoApi.Model.DomainExceprions;

namespace GeoApi.UnitTest;

public class NameTest
{
    public static IEnumerable<object[]> ValidNames =>
    new List<object[]>
    {
        new object[] { "JohnDoe" },
        new object[] { "Anna" },
        new object[] { "12345" },
        new object[] { "M4509"},
        new object[] { "f092"}
    };

    public static IEnumerable<object[]> InvalidNames =>
    new List<object[]>
    {
        new object[] { "Роб", "Ошибка в имени" },
        new object[] { "Anna Doe", "Ошибка в имени" },
        new object[] { "", "Имя не может быть пустым или отсутствовать" },
        new object[] { "M45_093", "Ошибка в имени"},
        new object[] { "f092!;", "Ошибка в имени"},
        new object[] { "рог;", "Ошибка в имени"},
        new object[] { "()", "Длина имени от 3 до 30 сиволов"},
        new object[] { "-+вввввв", "Ошибка в имени"},
        new object[] { "      ", "Имя не может быть пустым или отсутствовать"},
        new object[] { "d", "Длина имени от 3 до 30 сиволов"},
        new object[] { "-+lllllllllllllllllllllllllllllllllllllllllllllllllllllllll",
            "Длина имени от 3 до 30 сиволов" }
    };

    [Theory]
    [MemberData(nameof(ValidNames))]
    public void CreateName_ShouldBeValid(string name)
    {
        //act
        var validName = Name.CreateName(name);

        //assert
        Assert.NotNull(validName);
        Assert.Equal(validName.Value, name);
    }

    [Theory]
    [MemberData(nameof(InvalidNames))]
    public void CreateName_MustThrowException(string name, string expectedMessage)
    {
        //act
        var exception = Assert.Throws<NameException>(() => Name.CreateName(name));

        //assert
        Assert.IsType<NameException>(exception);
        Assert.Equal(exception.Message, expectedMessage);
    }

}