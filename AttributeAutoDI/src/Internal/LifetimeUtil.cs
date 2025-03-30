using System.Reflection;
using AttributeAutoDI.Attribute;

namespace AttributeAutoDI.Internal;

public static class LifetimeUtil
{
    public static ServiceLifetime? GetLifetimeFromAttributes(MemberInfo member)
    {
        if (member.GetCustomAttribute<SingletonAttribute>() != null)
            return ServiceLifetime.Singleton;
        if (member.GetCustomAttribute<ScopedAttribute>() != null)
            return ServiceLifetime.Scoped;
        if (member.GetCustomAttribute<TransientAttribute>() != null)
            return ServiceLifetime.Transient;

        return null;
    }
}