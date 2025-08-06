using Name.Bayfaderix.Darxxemiyur.Abstract;

namespace Name.Bayfaderix.Darxxemiyur.DataSource;

/// <summary>
/// Relation of one entity referencing many generically typed entities."/>
/// </summary>
public sealed class ReferencesToMany<TMany> : IEntityDataReference where TMany : Entity<TMany>, IIdentifiable<Entity<TMany>>
{
    Type IEntityDataReference.ThisType => typeof(ReferencesToMany<TMany>);

    Type IEntityDataReference.GenericType => typeof(TMany);

    Type IEntityDataReference.ParentType => throw new NotImplementedException();

    ReferenceType IEntityDataReference.ReferenceType => ReferenceType.ReferenceToMany;

    
}
