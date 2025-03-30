using System.Reflection;
using AttributeAutoDI.Internal.AttributeInjection;
using AttributeAutoDI.Internal.NameInjection;
using AttributeAutoDI.Internal.PrimaryInjection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AttributeAutoDI.Internal;

public static class AttributeInjectionExtension
{
    public static void AddAttributeDependencyInjection(
        this IServiceCollection services,
        Assembly? assembly = null
    )
    {
        assembly ??= Assembly.GetEntryAssembly() ?? typeof(Program).Assembly;

        services.AddControllersWithViews();
        services.AddControllers();
        services.UseAttributeInjection(assembly);
        services.UsePrimaryInjection();
        services.UseNameParameterInjection(assembly);
        services.Replace(ServiceDescriptor.Transient<IControllerActivator, NamedControllerActivator>());
    }
}