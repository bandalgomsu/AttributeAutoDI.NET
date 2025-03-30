using System.Reflection;
using AttributeAutoDI.Attribute;
using AttributeAutoDI.Internal.NameInjection;

namespace AttributeAutoDI.Internal.AttributeInjection;

public static class AttributeInjection
{
    public static void UseAttributeInjection(this IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(t =>
                t.GetCustomAttribute<SingletonAttribute>() != null ||
                t.GetCustomAttribute<ScopedAttribute>() != null ||
                t.GetCustomAttribute<TransientAttribute>() != null
            );

        foreach (var implType in types)
        {
            var serviceTypes = implType.GetInterfaces().Length != 0
                ? implType.GetInterfaces()
                : implType.BaseType != typeof(object)
                    ? new[] { implType.BaseType! }
                    : new[] { implType };

            var lifetime = LifetimeUtil.GetLifetimeFromAttributes(implType);
            if (lifetime == null) continue;

            var name = NameResolver.ResolveName(implType, lifetime.Value);

            if (implType.GetCustomAttribute<PrimaryAttribute>() != null)
            {
                foreach (var serviceType in serviceTypes)
                {
                    PrimaryInjection.PrimaryInjection.Add(serviceType, lifetime.Value, name, implType);
                    Console.WriteLine($"[AttributeAutoDI ⭐] Primary registered {serviceType.Name} → {implType.Name}");
                }

                continue;
            }


            foreach (var serviceType in serviceTypes)
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
                    case null:
                        break;
                }

                NameRegister.Register(serviceType, name, implType);

                Console.WriteLine($"[AttributeAutoDI ✅] Registered {serviceType.Name} → {implType.Name} as {lifetime}");
            }
        }
    }
}