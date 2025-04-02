using System.Reflection;
using AttributeAutoDI.Attribute;

namespace AttributeAutoDI.Internal.Configuration;

public static class PostConfiguration
{
    public static void UsePreConfiguration(this IServiceCollection services, Assembly assembly)
    {
        ExecuteMethodsWithAttribute<PreConfigurationAttribute>(services, assembly, "[Pre]");
    }

    public static void UsePostConfiguration(this IServiceCollection services, Assembly assembly)
    {
        ExecuteMethodsWithAttribute<PostConfigurationAttribute>(services, assembly, "[Post]");
    }

    private static void ExecuteMethodsWithAttribute<TAttr>(IServiceCollection services, Assembly assembly, string tag)
        where TAttr : System.Attribute
    {
        var types = assembly.GetTypes()
            .Where(t =>
                t is { IsAbstract: true, IsSealed: true }
                && t.GetCustomAttribute<TAttr>() != null);

        foreach (var type in types)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m =>
                    m.GetCustomAttribute<ExecuteAttribute>() != null &&
                    m.GetParameters().Length == 1 &&
                    m.GetParameters()[0].ParameterType == typeof(IServiceCollection));

            foreach (var method in methods)
            {
                method.Invoke(null, new object[] { services });

                Console.WriteLine($"[AttributeAutoDI ⚙️] {tag}Configuration executed: {type.Name}.{method.Name}()");
            }
        }
    }
}