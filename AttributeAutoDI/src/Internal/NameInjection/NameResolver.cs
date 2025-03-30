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
                                         ToCamelCase(implType.Name),
            ServiceLifetime.Scoped => implType.GetCustomAttribute<ScopedAttribute>()?.Name ??
                                      ToCamelCase(implType.Name),
            ServiceLifetime.Transient => implType.GetCustomAttribute<TransientAttribute>()?.Name ??
                                         ToCamelCase(implType.Name),
            _ => ToCamelCase(implType.Name)
        };
    }

    private static string ToCamelCase(string name)
    {
        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }
}