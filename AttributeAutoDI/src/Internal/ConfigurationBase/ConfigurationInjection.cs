// using System.Reflection;
// using AttributeAutoDI.Attribute;
//
// namespace AttributeAutoDI.Internal;
//
// public static class ConfigurationInjection
// {
//     public static void UseConfigurationInjection(
//         this IServiceCollection services,
//         Assembly assembly)
//     {
//         var configTypes = assembly.GetTypes()
//             .Where(type => type is { IsClass: true, IsAbstract: true, IsSealed: true }) // static class
//             .Where(type => type.GetCustomAttribute<ConfigurationAttribute>() != null);
//
//         foreach (var configClass in configTypes)
//         {
//             var methods = configClass.GetMethods(BindingFlags.Public | BindingFlags.Static)
//                 .Where(methodInfo =>
//                     methodInfo.GetParameters().Length == 1 &&
//                     methodInfo.GetParameters()[0].ParameterType == typeof(IServiceCollection) &&
//                     methodInfo.ReturnType == typeof(IServiceCollection)
//                 );
//
//             foreach (var method in methods)
//             {
//                 var lifetime = LifetimeUtil.GetLifetimeFromAttributes(method);
//                 if (lifetime == null) continue;
//
//                 var beforeCount = services.Count;
//
//                 method.Invoke(null, new object[] { services });
//
//                 var added = services.Skip(beforeCount).ToList();
//
//                 foreach (var sd in added)
//                     if (sd.Lifetime != lifetime.Value)
//                         throw new InvalidOperationException(
//                             $"Method '{method.Name}' is marked as [{lifetime}] but registered '{sd.ServiceType.Name}' as {sd.Lifetime}.");
//             }
//         }
//     }
// }

