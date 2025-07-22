using GeoApi.Model.DomainExceprions;
using NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using Coordinate = NetTopologySuite.Geometries.Coordinate;

namespace GeoApi.Model;

public class LocationData
{
    public Coordinate Centeroid { get; private set; }
    public Polygon Polygon { get; private set; }

    private LocationData(Coordinate center, Polygon polygon)
    {
        Centeroid = center;
        Polygon = polygon;
    }

    public static LocationData CreateLocationData(ICollection<Coordinate> coordinates)
    {
        //TODO валилдация обсудить с экспертом, аналитиком
        if (!coordinates.Any() || coordinates.Count < 3)
            throw new LocationExceprions("Требуется минимум три координаты");

        var ring = EnsureClosedRing(coordinates);

        var polygon = new Polygon(new LinearRing(ring.ToArray()));

        if (!polygon.IsValid)
            throw new LocationExceprions("Полигон не верен");

        var center = CalculateCenter(polygon);



        return new LocationData(center, polygon);
    }

    private static Coordinate CalculateCenter(Polygon polygon)
    {
        var longtitude = Math.Round(polygon.Centroid.Coordinate.X, 13);
        var latitude = Math.Round(polygon.Centroid.Coordinate.Y, 13);
        return new Coordinate(longtitude, latitude);
    }

    private static ICollection<Coordinate> EnsureClosedRing(ICollection<Coordinate> coordinates)
    {
        double tolerance = 1e-8;
        var first = coordinates.First();
        var last = coordinates.Last();

        bool isClosed = Math.Abs(first.X - last.X) < tolerance &&
                        Math.Abs(first.Y - last.Y) < tolerance;

        if (!isClosed)
        {
            coordinates.Add(new Coordinate(first.X, first.Y));
        }

        return coordinates;
    }

    public int CalculateAreaInHectares()
    {
        //TODO вариант расчета
        var wgsCoords = Polygon.Coordinates;
        double avgLat = wgsCoords.Average(c => c.Y);
        double avgLon = wgsCoords.Average(c => c.X);


        int utmZone = (int)Math.Floor((avgLon + 180) / 6) + 1;
        bool isNorthern = avgLat >= 0;


        var wgs84 = GeographicCoordinateSystem.WGS84;
        var utm = ProjectedCoordinateSystem.WGS84_UTM(utmZone, isNorthern);
        var transform = new CoordinateTransformationFactory()
            .CreateFromCoordinateSystems(wgs84, utm)
            .MathTransform;


        var utmCoords = wgsCoords
            .Select(c =>
            {
                var pfg = transform.Transform([c.X, c.Y]);
                return new Coordinate(pfg[0], pfg[1]);
            })
            .ToArray();


        var utmPolygon = new Polygon(new LinearRing(utmCoords));
        double areaSqMeters = utmPolygon.Area;
        return (int)Math.Round(areaSqMeters / 10_000.0);
    }

    public int CalculateAreaInHectares2()
    {
        return (int)Math.Round(Polygon.Area * 1000000);
    }



}
