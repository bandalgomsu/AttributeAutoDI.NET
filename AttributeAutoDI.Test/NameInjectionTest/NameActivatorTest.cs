using AttributeAutoDI.Attribute;
using AttributeAutoDI.Internal.NameInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Test.NameInjectionTest;

[Collection("AttributeAutoDI.Test")]
public class NamedActivatorTest
{
    public NamedActivatorTest()
    {
        NameRegister.Clear();
    }

    [Fact]
    public void Should_Create_Instance_With_Named_Implementation()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IAnimal, Cat>();
        services.AddSingleton<IAnimal, Dog>();

        NameRegister.Register(typeof(IAnimal), "cat", typeof(Cat));

        var provider = services.BuildServiceProvider();

        var owner = (PetOwner)NamedActivator.CreateInstance(provider, typeof(PetOwner));

        Assert.NotNull(owner);
        Assert.IsType<Cat>(owner.Pet);
        Assert.Equal("Meow", owner.Pet.Speak());
    }

    [Fact]
    public void Should_Throw_When_Named_Implementation_Not_Found()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IAnimal, Dog>(); // Cat 안 넣음

        NameRegister.Register(typeof(IAnimal), "cat", typeof(Cat));

        var provider = services.BuildServiceProvider();

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            NamedActivator.CreateInstance(provider, typeof(PetOwner));
        });

        Assert.Contains("No service found for type", ex.Message);
    }

    [Fact]
    public void Should_Fallback_To_Regular_Resolution_If_No_Named_Attribute()
    {
        var services = new ServiceCollection();
        services.AddSingleton<SimpleService>();

        var provider = services.BuildServiceProvider();

        var instance = (SimpleConsumer)NamedActivator.CreateInstance(provider, typeof(SimpleConsumer));

        Assert.NotNull(instance);
        Assert.Equal("Hello", instance.Service.Greet());
    }
}

public class SimpleService
{
    public string Greet()
    {
        return "Hello";
    }
}

public class SimpleConsumer(SimpleService service)
{
    public SimpleService Service => service;
}

public interface IAnimal
{
    string Speak();
}

public class Cat : IAnimal
{
    public string Speak()
    {
        return "Meow";
    }
}

public class Dog : IAnimal
{
    public string Speak()
    {
        return "Woof";
    }
}

public class PetOwner([Named("cat")] IAnimal animal)
{
    public IAnimal Pet => animal;
}