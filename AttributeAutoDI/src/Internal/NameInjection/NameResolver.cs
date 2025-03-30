using System.Reflection;
using AttributeAutoDI.Attribute;

namespace AttributeAutoDI.Internal.NameInjection;

public static class NameResolver
{
    public static string ResolveName(Type implType, ServiceLifetime lifetime)
    {
        return lifetime switch
        {
            ServiceLifetime.Singleton => implType.GetCustomAttribute<SingletonAttribute>()?.Name ??
                                         ToLowerCamelCase(implType.Name),
            ServiceLifetime.Scoped => implType.GetCustomAttribute<ScopedAttribute>()?.Name ??
                                      ToLowerCamelCase(implType.Name),
            ServiceLifetime.Transient => implType.GetCustomAttribute<TransientAttribute>()?.Name ??
                                         ToLowerCamelCase(implType.Name),
            _ => ToLowerCamelCase(implType.Name)
        };
    }

    private static string ToLowerCamelCase(string name)
    {
        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }
}