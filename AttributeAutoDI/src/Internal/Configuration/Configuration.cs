using System.Reflection;
using AttributeAutoDI.Attribute;

namespace AttributeAutoDI.Internal.Configuration;

public static class Configuration
{
    public static void UsePreConfiguration(this IServiceCollection services, IConfiguration configuration,
        Assembly assembly)
    {
        ExecuteMethodsWithAttribute<PreConfigurationAttribute>(services, assembly, configuration, "[Pre]");
    }

    public static void UsePostConfiguration(this IServiceCollection services, IConfiguration configuration,
        Assembly assembly)
    {
        ExecuteMethodsWithAttribute<PostConfigurationAttribute>(services, assembly, configuration, "[Post]");
    }

    private static void ExecuteMethodsWithAttribute<TAttr>(IServiceCollection services, Assembly assembly,
        IConfiguration configuration, string tag)
        where TAttr : System.Attribute
    {
        var types = assembly.GetTypes()
            .Where(t =>
                t is { IsAbstract: true, IsSealed: true }
                && t.GetCustomAttribute<TAttr>() != null);

        foreach (var type in types)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.GetCustomAttribute<ExecuteAttribute>() != null);

            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                var args = new object?[parameters.Length];

                for (var i = 0; i < parameters.Length; i++)
                {
                    var paramType = parameters[i].ParameterType;
                    if (paramType == typeof(IServiceCollection))
                    {
                        args[i] = services;
                    }
                    else if (paramType == typeof(IConfiguration))
                    {
                        args[i] = configuration;
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"[AttributeAutoDI ❌] Unsupported parameter type: {paramType.Name} in {method.Name}()");
                    }
                }

                method.Invoke(null, args);
                Console.WriteLine($"[AttributeAutoDI ⚙️] {tag}Configuration executed: {type.Name}.{method.Name}()");
            }
        }
    }
}