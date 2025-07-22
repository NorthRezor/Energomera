namespace GeoApi.Model.DomainExceprions;

public class NameException : Exception
{
    public NameException()
    { }

    public NameException(string message)
        : base(message)
    { }
}
