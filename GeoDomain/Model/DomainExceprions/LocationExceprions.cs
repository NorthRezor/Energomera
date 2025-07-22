namespace GeoApi.Model.DomainExceprions;

public class LocationExceprions : Exception
{
    public LocationExceprions()
    { }

    public LocationExceprions(string message)
        : base(message)
    { }
}
