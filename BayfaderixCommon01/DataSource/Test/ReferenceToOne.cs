using Name.Bayfaderix.Darxxemiyur.Abstract;

namespace Name.Bayfaderix.Darxxemiyur.DataSource;

/// <summary>
/// Relation of an entity to one generically typed entity.
/// </summary>
public sealed class ReferenceToOne<TOne> : IEntityDataReference where TOne : Entity<TOne>, IIdentifiable<Entity<TOne>>
{
    Type IEntityDataReference.ThisType => typeof(ReferenceToOne<TOne>);

    Type IEntityDataReference.GenericType => typeof(TOne);

    Type IEntityDataReference.ParentType => throw new NotImplementedException();

    ReferenceType IEntityDataReference.ReferenceType => ReferenceType.ReferenceToMany;
}