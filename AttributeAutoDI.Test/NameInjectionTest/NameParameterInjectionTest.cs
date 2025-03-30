using AttributeAutoDI.Attribute;
using AttributeAutoDI.Internal.NameInjection;
using AttributeAutoDI.Internal.PrimaryInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Test.NameInjectionTest;

[Collection("AttributeAutoDI.Test")]
public class NameParameterInjectionTest
{
    public NameParameterInjectionTest()
    {
        NameRegister.Clear();
        PrimaryInjection.Clear();
    }

    [Fact]
    public void Named_Parameter_Should_Be_Injected_Properly()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IColorService, RedColorService>();
        NameRegister.Register(typeof(IColorService), "red", typeof(RedColorService));

        services.UseNameParameterInjection(typeof(ColorPrinter).Assembly);

        var provider = services.BuildServiceProvider();

        var printer = provider.GetRequiredService<ColorPrinter>();
        var result = printer.Print();

        Assert.Equal("Red", result); // red가 주입되어야 함
    }
}

public interface IColorService
{
    string GetColor();
}

[Singleton("red")]
public class RedColorService : IColorService
{
    public string GetColor()
    {
        return "Red";
    }
}

[Singleton]
public class ColorPrinter([Named("red")] IColorService colorService)
{
    public string Print()
    {
        return colorService.GetColor();
    }
}