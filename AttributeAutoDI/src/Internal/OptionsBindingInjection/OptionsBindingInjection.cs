using System.Reflection;
using AttributeAutoDI.Attribute;

namespace AttributeAutoDI.Internal.OptionsBindingInjection;

public static class OptionsBindingInjection
{
    public static void UseOptionsBindingInjection(this IServiceCollection services, IConfiguration configuration,
        Assembly assembly)
    {
        var optionTypes = assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<OptionsAttribute>() is not null);

        foreach (var type in optionTypes)
        {
            var attr = type.GetCustomAttribute<OptionsAttribute>()!;
            var section = configuration.GetSection(attr.Section);

            var method = typeof(OptionsConfigurationServiceCollectionExtensions)
                .GetMethods()
                .First(m => m.Name == "Configure" && m.GetGenericArguments().Length == 1 &&
                            m.GetParameters().Length == 2);

            var generic = method.MakeGenericMethod(type);
            generic.Invoke(null, new object[] { services, section });

            Console.WriteLine($"[AttributeAutoDI 🛠] Options bound:  {attr.Section} -> {type.Name}");
        }
    }
}