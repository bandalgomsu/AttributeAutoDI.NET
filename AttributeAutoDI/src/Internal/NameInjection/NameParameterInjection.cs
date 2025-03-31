using System.Reflection;
using AttributeAutoDI.Attribute;
using Microsoft.AspNetCore.Mvc;

namespace AttributeAutoDI.Internal.NameInjection;

public static class NameParameterInjection
{
    public static void UseNameParameterInjection(this IServiceCollection services, Assembly assembly)
    {
        var candidates = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(t => t.GetConstructors()
                .Any(c => c.GetParameters().Any(p => p.GetCustomAttribute<NamedAttribute>() != null)));

        foreach (var type in candidates)
        {
            var descriptor = services.FirstOrDefault(sd => sd.ServiceType == type || sd.ImplementationType == type);

            var lifetime = descriptor?.Lifetime;

            lifetime ??= LifetimeUtil.GetLifetimeFromAttributes(type);

            var isController = typeof(ControllerBase).IsAssignableFrom(type);
            if (lifetime == null && isController)
            {
                lifetime = ServiceLifetime.Transient;
                Console.WriteLine($"[AttributeAutoDI ⚙️] Controller {type.Name} auto-registered as Transient");
            }

            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton(type, sp => NamedActivator.CreateInstance(sp, type));
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped(type, sp => NamedActivator.CreateInstance(sp, type));
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient(type, sp => NamedActivator.CreateInstance(sp, type));
                    break;
                default:
                    throw new InvalidOperationException(
                        $"[AttributeAutoDI ❌] Cannot determine lifetime for '{type.FullName}'. " +
                        $"Make sure it's either registered already or decorated with [Singleton]/[Scoped]/[Transient].");
            }

            Console.WriteLine($"[AttributeAutoDI ✅] NamedConsumer registered: {type.Name} as {lifetime}");
        }
    }
}