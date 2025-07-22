using GeoApi.Model;
using NetTopologySuite.Geometries;

namespace GeoApi.UnitTest;

public class FieldTest
{
    private readonly LocationData _locationData;
    private readonly Name _name;
    public FieldTest()
    {
        var coordinates = new List<Coordinate>
        {
            new Coordinate(41.3346809239899, 45.7074047669366),
            new Coordinate(41.3414148034017, 45.707543073278),
            new Coordinate(41.3414148034017, 45.6850638023809),
            new Coordinate(41.3347304378091, 45.6849600309502),
            new Coordinate(41.3346809239899, 45.7074047669366)
        };

        _locationData = LocationData.CreateLocationData(coordinates);
        _name = Name.CreateName("TestName");
    }
    [Fact]
    public void CreateField_ShouldSetCorrectSize()
    {
        //arrange
        var size = 151;

        //act
        var field = Field.CreateField(_name, _locationData);

        //assert
        Assert.Equal(size, field.Size);
    }

    [Fact]
    public void CreateField_CorrectName()
    {
        //act
        var field = Field.CreateField(_name, _locationData);

        //assert
        Assert.NotNull(field.Name);
        Assert.Equal(_name.Value, field.Name.ToString());
    }
}
