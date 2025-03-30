using System.Reflection;
using AttributeAutoDI.Attribute;
using AttributeAutoDI.Internal.NameInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Test.NameInjectionTest;

[Collection("AttributeAutoDI.Test")]
public class NamedControllerActivatorTest
{
    public NamedControllerActivatorTest()
    {
        NameRegister.Clear();
    }

    [Fact]
    public void Should_Create_Controller_With_Named_Parameter()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMessageService, KoreanMessageService>();
        NameRegister.Register(typeof(IMessageService), "english", typeof(KoreanMessageService));

        var provider = services.BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = provider
        };

        var controllerType = typeof(GreetingController);

        var controllerContext = new ControllerContext
        {
            HttpContext = httpContext,
            ActionDescriptor = new ControllerActionDescriptor
            {
                ControllerTypeInfo = controllerType.GetTypeInfo()
            },
            RouteData = new RouteData()
        };

        var activator = new NamedControllerActivator();

        var controller = activator.Create(controllerContext) as GreetingController;

        Assert.NotNull(controller);
        Assert.Equal("HI", controller.Greet());
    }

    [Fact]
    public void Release_Should_Not_Throw()
    {
        var activator = new NamedControllerActivator();

        var context = new ControllerContext();
        var dummyController = new GreetingController(new KoreanMessageService());

        // Act & Assert: Should not throw
        activator.Release(context, dummyController);
    }
}

public interface IMessageService
{
    string Message();
}

[Singleton("english")]
public class KoreanMessageService : IMessageService
{
    public string Message()
    {
        return "HI";
    }
}

public class GreetingController([Named("english")] IMessageService service) : ControllerBase
{
    public string Greet()
    {
        return service.Message();
    }
}