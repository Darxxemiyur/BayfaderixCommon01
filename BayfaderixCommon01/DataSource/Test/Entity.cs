using Name.Bayfaderix.Darxxemiyur.Abstract;
using Name.Bayfaderix.Darxxemiyur.Collections;
using System.Reflection;

namespace Name.Bayfaderix.Darxxemiyur.DataSource;

public abstract class Entity<TImplementingType> where TImplementingType : Entity<TImplementingType>, IIdentifiable<Entity<TImplementingType>>
{
    public delegate TOut Selector<TOut>(TImplementingType selectant);
    internal IEnumerable<IEntityDataReference> EntityDataReferences { get => this.GetReflectedEDRs(); }
}

internal static class EntityReflector
{
    private static readonly Dictionary<Type, (FieldInfo[] fi, PropertyInfo[] pi)> _typeFields = new();

    internal static (FieldInfo[] fi, PropertyInfo[] pi) GetFields(Type type)
    {
        if (_typeFields.TryGetValue(type, out var pairs))
            return pairs;

        var members = type.GetFields().Where(x => x.FieldType == typeof(IEntityDataReference)).ToArray();
        var properties = type.GetProperties().Where(x => x.GetMethod?.ReturnType == typeof(IEntityDataReference)).ToArray();

        _typeFields.Add(type, new(members, properties));

        return GetFields(type);
    }

    internal static IReadOnlyCollection<IEntityDataReference> GetReflectedEDRs<TImplementingType>(this Entity<TImplementingType> entity) where TImplementingType : Entity<TImplementingType>, IIdentifiable<Entity<TImplementingType>>
    {
        var (fields, properties) = GetFields(entity.GetType());

        var fieldInstances = fields.Select(x => (IEntityDataReference)x.GetValue(entity)!);
        var propertyInstances = properties.Select(x => (IEntityDataReference)x.GetValue(entity)!);

        return fieldInstances.Concat(propertyInstances).ToLinkedList();

    }
}
