namespace OrderService.Domain.ValueObjects;

public abstract class ValueObject
{
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right) => !(left == right);

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        var valueObject = (ValueObject)obj;
        return GetAtomicValues().SequenceEqual(valueObject.GetAtomicValues());
    }

    public override int GetHashCode()
    {
        return GetAtomicValues()
            .Aggregate(default(int), (hashcode, value) =>
            {
                unchecked
                {
                    return hashcode * 397 ^ (value?.GetHashCode() ?? 0);
                }
            });
    }

    protected abstract IEnumerable<object?> GetAtomicValues();
}
