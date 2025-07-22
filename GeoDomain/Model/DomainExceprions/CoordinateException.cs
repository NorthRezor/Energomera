namespace GeoApi.Model.DomainExceprions;

public class CoordinateException : Exception
{
    public CoordinateException()
    { }

    public CoordinateException(string message)
        : base(message)
    { }
}
