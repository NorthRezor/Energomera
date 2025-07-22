using System.Globalization;
using System.Reflection;
using GeoApi.Model;
using NetTopologySuite.Geometries;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;

namespace GeoApi.Infrastructure;

public class ParserFromFileService
{
    public static List<Field> ParseKml(string fieldsString, string centroidsString)
    {
        var parser = new Parser();
        var fields = new List<Field>();
        var parserFields = new List<ParserField>();
        var parserCentroid = new List<ParserCentroid>();

        var placemarksField = TakePlacemarks(parser, fieldsString);

        foreach (var placemark in placemarksField)
        {
            string name = placemark.Name;
            int fid = 0, size = 0;

            var schemaData = placemark.ExtendedData?.SchemaData?.FirstOrDefault();

            if (schemaData != null)
            {
                foreach (var data in schemaData.SimpleData)
                {
                    if (data.Name == "fid")
                        fid = Convert.ToInt32(float.Parse(data.Text, CultureInfo.InvariantCulture));
                    if (data.Name == "size")
                        size = Convert.ToInt32(float.Parse(data.Text, CultureInfo.InvariantCulture));
                }
            }

            var poly = placemark.Geometry as SharpKml.Dom.Polygon;
            var coordList = poly?.OuterBoundary?.LinearRing?.Coordinates;

            var wgsCoords = coordList!
                .Select(c => new Coordinate(c.Longitude, c.Latitude))
                .ToList();

            var ntsPolygon = new NetTopologySuite.Geometries
                .Polygon(new NetTopologySuite.Geometries.LinearRing(wgsCoords.ToArray()));


            parserFields.Add(new()
            {
                Id = fid,
                Name = name,
                Size = size,
                Polygon = ntsPolygon

            });


        }

        var placemarksCentroids = TakePlacemarks(parser, centroidsString);

        foreach (var placemark in placemarksCentroids)
        {
            string name = placemark.Name;
            int fid = 0;

            var schemaData = placemark.ExtendedData?.SchemaData?.FirstOrDefault();

            if (schemaData != null)
            {
                foreach (var data in schemaData.SimpleData)
                {
                    if (data.Name == "fid")
                        fid = Convert.ToInt32(float.Parse(data.Text, CultureInfo.InvariantCulture));
                }
            }

            var poly = placemark.Geometry as SharpKml.Dom.Point;
            var coordList = poly?.Coordinate ?? new Vector(0, 0);

            var centroid = new Coordinate(coordList.Longitude, coordList.Latitude);

            parserCentroid.Add(new(fid, centroid));


        }

        foreach (var field in parserFields)
        {
            foreach (var centroid in parserCentroid)
            {
                if (field.Id == centroid.Id)
                    field.Centroid = centroid.Coordinate;
            }
        }

        //TODO для примера рефлексия, чтобы не ломать модель,  реальности загрузка будет из базы.

        foreach (var field in parserFields)
        {
            var ctorLocationData = typeof(LocationData)
            .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .FirstOrDefault(c => c.GetParameters().Length == 2);

            var locationData = (LocationData)ctorLocationData!.Invoke([field.Centroid, field.Polygon]);

            var ctorField = typeof(Field)
            .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .FirstOrDefault(c => c.GetParameters().Length == 3);

            var fieldToAdd = (Field)ctorField!.Invoke([Name.CreateName(field.Name!), field.Size, locationData]);
            var addId = typeof(Field)
                .GetProperty("Id");

            addId!.SetValue(fieldToAdd, field.Id);

            fields.Add(fieldToAdd);

        }

        return fields;
    }

    private static IEnumerable<Placemark> TakePlacemarks(Parser parser, string text)
    {
        parser.ParseString(text, false);

        var kml = parser.Root as Kml;
        return kml?.Flatten().OfType<Placemark>() ?? [];
    }

    private record ParserCentroid(int Id, Coordinate Coordinate);
    private class ParserField
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Size { get; set; }
        public NetTopologySuite.Geometries.Polygon? Polygon { get; set; }
        public Coordinate? Centroid { get; set; }
    }
}
