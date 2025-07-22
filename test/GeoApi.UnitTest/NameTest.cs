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
        new object[] { "–Ó·", "Name invalid" },
        new object[] { "Anna Doe", "Name invalid" },
        new object[] { "", "Name can't be null or empty" },
        new object[] { "M45_093", "Name invalid"},
        new object[] { "f092!;", "Name invalid"},
        new object[] { "Ó„;", "Name invalid"},
        new object[] { "()", "Name length must be between 3 and 30 characters"},
        new object[] { "-+‚‚‚‚‚‚", "Name invalid"},
        new object[] { "      ", "Name can't be null or empty"},
        new object[] { "d", "Name length must be between 3 and 30 characters"},
        new object[] { "-+lllllllllllllllllllllllllllllllllllllllllllllllllllllllll",
            "Name length must be between 3 and 30 characters" }
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