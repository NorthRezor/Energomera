using GeoApi.Model;

namespace GeoApi.Infrastructure;

public static class InfDependencyInjection
{
    public static (string, string) ReadFromFile(string? pathFields = null, string? pathCentroids = null)
    {
        var fields = File.ReadAllText(@"Infrastructure/Resourse/fields.kml");
        var centroids = File.ReadAllText(@"Infrastructure/Resourse/centroids.kml");

        return (fields, centroids);

    }

    public static ICollection<Field> ParseData(string fields, string centroids)
    {
        return ParserFromFileService.ParseKml(fields, centroids);
    }

    public static IServiceCollection AddDataStorage(this IServiceCollection services)
    {
        var data = ReadFromFile();
        var fields = ParseData(data.Item1, data.Item2);
        services.AddSingleton(new DataStorageService(fields));
        return services;
    }


}
