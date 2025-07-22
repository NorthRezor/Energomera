using GeoApi.Infrastructure;

namespace GeoApi;

public record GeoServices(DataStorageService Storage, ILogger<GeoServices> Logger);
