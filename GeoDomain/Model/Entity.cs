namespace GeoDomain.Model;

public abstract class Entity
{
    private int _Id;
    public virtual int Id
    {
        get
        {
            return _Id;
        }
        protected set
        {
            _Id = value;
        }
    }
    public bool IsDefault()
    {
        return Id == default;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj is null || GetType() != obj.GetType())
            return false;

        var other = (Entity)obj;

        if (IsDefault() || other.IsDefault())
            return false;

        return Id.Equals(other.Id);
    }

    public override int GetHashCode() => HashCode.Combine(Id);
}