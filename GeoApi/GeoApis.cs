using GeoApi.ApiDto;
using GeoApi.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;

namespace GeoApi;

public static class GeoApis
{
    public static IEndpointRouteBuilder MapGeoApi(this IEndpointRouteBuilder routeBuilder)
    {
        var api = routeBuilder.MapGroup("api/geo");

        api.MapGet("/fields", GetAllFields);
        api.MapGet("/fields/{id:int}/size", GetFieldSize);
        api.MapGet("/fields/{id:int}/distance", GetDistanceFromCenterToPoint);
        api.MapGet("/fields/contains-point", BelongPointToField);

        return api;
    }

    //TODO методы синхронные т.к. мы берем данные из кеша всегда в который заносим данные при старте приложения
    //если данные брать из базы методы будут асинхронные. Собственно это обсуждалось при уточнии ТЗ, что база не нужна.
    public static IResult GetAllFields([AsParameters] GeoServices services)
    {

        var fields = services.Storage.Fields.Values;

        var result = fields.Select(f => new QueryField(f.Id,
                                                     f.Name.ToString(),
                                                     new QueryLocation([f.Locations.Centeroid.Y, f.Locations.Centeroid.X],
                                                     f.Locations.Polygon.Coordinates
                                                      .Select(c => new[] { c.Y, c.X })
                                                      .ToArray()))
                                                     ).ToList();

        //if (!result.Any())
        //{
        //    return TypedResults.Ok<ICollection<QueryField>>(new List<QueryField>());
        //}

        return TypedResults.Ok<ICollection<QueryField>>(result);

    }

    public static Results<Ok<int>, NotFound> GetFieldSize([AsParameters] GeoServices services,
        int id)
    {
        if (!services.Storage.Fields.TryGetValue(id, out Field? value) || value is not Field field)
        {
            services.Logger.LogInformation("Поле отсутствует");
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(field.Size);

    }


    public static Results<Ok<int>, NotFound, BadRequest<string>> GetDistanceFromCenterToPoint([AsParameters] GeoServices services,
        int id, [FromQuery] double lat, [FromQuery] double lng)
    {

        if (!services.Storage.Fields.TryGetValue(id, out Field? value) || value is not Field field)
        {
            //TODO пример логов
            services.Logger.LogInformation("Поле отсутствует");
            return TypedResults.NotFound();
        }

        if (lat == default || lng == default)
        {
            return TypedResults.BadRequest("Координаты не должны равняться нулю");
        }

        var coordinate = new Coordinate(lng, lat);

        if (!field.ContaintPoint(coordinate))
        {
            var error = "Точка находится вне поля";
            services.Logger.LogInformation(error);
            return TypedResults.BadRequest(error);
        }


        return TypedResults.Ok(field.DistanceFromCentrFieldToPoint(coordinate));

    }

    public static Results<Ok<QueryField>, NotFound<bool>> BelongPointToField([AsParameters] GeoServices services,
        [FromQuery] double lat, [FromQuery] double lng)
    {
        var fields = services.Storage.Fields.Values;
        if (!fields.Any())
            return TypedResults.NotFound(false);

        var field = fields.FirstOrDefault(f => f.ContaintPoint(new Coordinate(lng, lat)));
        if (field == null)
            return TypedResults.NotFound(false);

        var result = new QueryField(field.Id, field.Name.ToString(), new QueryLocation(Array.Empty<double>(), Array.Empty<double[]>()));

        return TypedResults.Ok(result);
    }


}
