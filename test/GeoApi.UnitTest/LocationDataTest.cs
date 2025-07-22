using GeoApi.Model;
using GeoApi.Model.DomainExceprions;
using NetTopologySuite.Geometries;

namespace GeoApi.UnitTest;

public class LocationDataTest
{
    [Fact]
    public void CreateLocationData_IncorrectCountOfCoordinate()
    {
        //arrange
        var coordinates = new List<Coordinate>
        {
            new Coordinate(41.3346809239899, 45.7074047669366),
            new Coordinate(41.3414148034017, 45.707543073278)
        };
        var exceptionMessage = "At least 3 coordinates required";

        //act
        var exception = Assert.Throws<LocationExceprions>(() => LocationData.CreateLocationData(coordinates));

        //assert
        Assert.IsType<LocationExceprions>(exception);
        Assert.Equal(exceptionMessage, exception.Message);


    }


    [Fact]
    public void CreateLocationData_InvalidPolygon()
    {
        //arrange
        var coordinates = new List<Coordinate>
        {
            new Coordinate(41.3346809239899, 45.7074047669366),
            new Coordinate(41.3414148034017, 45.707543073278),
            new Coordinate(41.3414148034017, 75.707543073278),
            new Coordinate(41.3414148034017, 45.6850638023809),
            new Coordinate(41.3347304378091, 45.6849600309502),
            new Coordinate(41.3346809239899, 45.7074047669366)
        };
        var exceptionMessage = "Polygon is invalid";

        //act
        var exception = Assert.Throws<LocationExceprions>(() => LocationData.CreateLocationData(coordinates));

        //assert
        Assert.IsType<LocationExceprions>(exception);
        Assert.Equal(exceptionMessage, exception.Message);


    }

    [Fact]
    public void Centroid_Calculate()
    {
        //arrange
        var coordinates = new List<Coordinate>
        {
            new Coordinate(41.3346809239899, 45.7074047669366),
            new Coordinate(41.3414148034017, 45.707543073278),
            new Coordinate(41.3414148034017, 45.6850638023809),
            new Coordinate(41.3347304378091, 45.6849600309502),
            new Coordinate(41.3346809239899, 45.7074047669366)
        };

        var centroid = new Coordinate(41.3380610642586, 45.6962567581079);

        //act

        var locationData = LocationData.CreateLocationData(coordinates);

        //assert

        Assert.Equal(centroid, locationData.Centeroid);
    }
}