using System.Reflection;
using AttributeAutoDI.Attribute;

namespace AttributeAutoDI.Internal.NameInjection;

public static class NamedActivator
{
    public static object CreateInstance(IServiceProvider provider, Type type)
    {
        var ctor = type.GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();
        if (ctor == null)
            throw new InvalidOperationException($"No public constructor found for type '{type.Name}'");

        var parameters = ctor.GetParameters();

        var args = parameters.Select(p =>
        {
            var named = p.GetCustomAttribute<NamedAttribute>();

            if (named == null) return provider.GetRequiredService(p.ParameterType);
            var implType = NameRegister.Resolve(p.ParameterType, named.Name);

            return InjectNamedInstance(provider, p.ParameterType, implType);
        }).ToArray();

        return ctor.Invoke(args);
    }

    private static object InjectNamedInstance(IServiceProvider provider, Type interfaceType, Type implementationType)
    {
        var services = provider.GetServices(interfaceType);

        foreach (var service in services)
            if (service?.GetType() == implementationType)
                return service;

        throw new InvalidOperationException($"No service found for type '{interfaceType.Name}'");
    }
}