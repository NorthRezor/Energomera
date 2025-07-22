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
        new object[] { "���", "������ � �����" },
        new object[] { "Anna Doe", "������ � �����" },
        new object[] { "", "��� �� ����� ���� ������ ��� �������������" },
        new object[] { "M45_093", "������ � �����"},
        new object[] { "f092!;", "������ � �����"},
        new object[] { "���;", "������ � �����"},
        new object[] { "()", "����� ����� �� 3 �� 30 �������"},
        new object[] { "-+������", "������ � �����"},
        new object[] { "      ", "��� �� ����� ���� ������ ��� �������������"},
        new object[] { "d", "����� ����� �� 3 �� 30 �������"},
        new object[] { "-+lllllllllllllllllllllllllllllllllllllllllllllllllllllllll",
            "����� ����� �� 3 �� 30 �������" }
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