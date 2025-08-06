namespace Name.Bayfaderix.Darxxemiyur.DataSource;

/// <summary>
/// A property that can be fetched on request.
/// Represents an entity that is kept in the same record. 
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class Property<T> : IEntityDataReference
{
    private T _value = default!;
    public DataChangeStatus Status { get; }

    Type IEntityDataReference.ThisType => typeof(Property<T>);

    Type IEntityDataReference.GenericType => typeof(T);

    Type IEntityDataReference.ParentType => throw new NotImplementedException();

    ReferenceType IEntityDataReference.ReferenceType => ReferenceType.Property;
}
[Flags]
public enum DataChangeStatus
{
    FullyLoaded = 1,
    PartiallyLoaded = FullyLoaded << 1,
    Modified = PartiallyLoaded << 1,
    Set = Modified << 1,
}