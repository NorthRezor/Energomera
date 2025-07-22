namespace GeoApi.Model.DomainExceprions;

public class FieldException : Exception
{
    public FieldException()
    { }

    public FieldException(string message)
        : base(message)
    { }
}
