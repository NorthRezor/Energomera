using GeoDomain.Model;
using NetTopologySuite.Geometries;

namespace GeoApi.Model;

public class Field : Entity
{
    public Name Name { get; set; } = null!;
    public int Size { get; private set; }
    public LocationData Locations { get; private set; } = null!;

    private Field()
    {

    }
    private Field(Name name, int size, LocationData loctionData)
    {
        Name = name;
        Size = size;
        Locations = loctionData;
    }

    public static Field CreateField(Name name, LocationData locationData)
    {
        //TODO предположим ид создается базой.
        //дополнительная валидация бизнес логики по запросу.
        var size = locationData.CalculateAreaInHectares2();


        return new Field(name, size, locationData);
    }

    public void ChangeLocations(LocationData locationData)
    {
        Locations = locationData;
        Size = locationData.CalculateAreaInHectares();
    }

    public bool ContaintPoint(Coordinate coordinate) =>
        Locations.Polygon.Contains(new Point(coordinate));

    public int DistanceFromCentrFieldToPoint(Coordinate coordinate)
    {
        // TODO Формула Хаверсина.
        static double ToRadians(double deg) => deg * Math.PI / 180;
        const double R = 6371000;

        double lat1 = ToRadians(Locations.Centeroid.Y);
        double lon1 = ToRadians(Locations.Centeroid.X);
        double lat2 = ToRadians(coordinate.Y);
        double lon2 = ToRadians(coordinate.X);

        double dLat = lat2 - lat1;
        double dLon = lon2 - lon1;

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1) * Math.Cos(lat2) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        double distance = R * c;

        return (int)Math.Round(distance);
    }
}
