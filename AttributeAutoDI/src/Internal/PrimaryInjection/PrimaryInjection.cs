using System.Collections.Concurrent;
using AttributeAutoDI.Internal.NameInjection;

namespace AttributeAutoDI.Internal.PrimaryInjection;

public static class PrimaryInjection
{
    private static readonly ConcurrentDictionary<Type, (ServiceLifetime Lifetime, string Name, Type implType)>
        PrimaryMap = new();

    public static void Add(Type serviceType, ServiceLifetime lifetime, string name, Type implType)
    {
        if (PrimaryMap.TryGetValue(serviceType, out var value))
            throw new InvalidOperationException(
                $"[AttributeAutoDI ❌] Multiple [Primary] registrations for {serviceType.Name}: Exist : {value.Name}, New : {implType.Name}");

        PrimaryMap[serviceType] = (lifetime, name, implType);
    }

    public static void Clear()
    {
        PrimaryMap.Clear();
    }

    public static IReadOnlyDictionary<Type, (ServiceLifetime Lifetime, string Name, Type implType)> GetMap()
    {
        return PrimaryMap;
    }

    public static void UsePrimaryInjection(this IServiceCollection services)
    {
        foreach (var (serviceType, (lifetime, name, implType)) in PrimaryMap)
        {
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton(serviceType, implType);
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped(serviceType, implType);
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient(serviceType, implType);
                    break;
            }

            NameRegister.Register(serviceType, name, implType);
        }

        PrimaryMap.Clear();
    }
}