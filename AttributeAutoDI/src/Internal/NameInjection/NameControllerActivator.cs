using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace AttributeAutoDI.Internal.NameInjection;

public class NamedControllerActivator : IControllerActivator
{
    public object Create(ControllerContext context)
    {
        var controllerType = context.ActionDescriptor.ControllerTypeInfo.AsType();
        var provider = context.HttpContext.RequestServices;

        return NamedActivator.CreateInstance(provider, controllerType);
    }

    public void Release(ControllerContext context, object controller)
    {
    }
}