namespace Name.Bayfaderix.Darxxemiyur.DataSource;

internal interface IEntityDataReference
{
    Type ThisType { get; } //This type
    Type GenericType { get; } //Referencable type
    Type ParentType { get; } //Housing type
    ReferenceType ReferenceType { get; }
}

internal enum ReferenceType
{
    Property, ReferenceToMany, ReferenceToOne
}

