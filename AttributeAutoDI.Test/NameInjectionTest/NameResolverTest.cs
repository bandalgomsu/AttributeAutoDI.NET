using AttributeAutoDI.Attribute;
using AttributeAutoDI.Internal.NameInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Test.NameInjectionTest;

[Collection("AttributeAutoDI.Test")]
public class NameResolverTests
{
    [Fact]
    public void Should_Use_Custom_Name_If_Provided()
    {
        var name = NameResolver.ResolveName(typeof(NamedSingletonService), ServiceLifetime.Singleton);
        Assert.Equal("customSingleton", name);
    }

    [Fact]
    public void Should_Fallback_To_CamelCase_If_Name_Is_Not_Provided_Scoped()
    {
        var name = NameResolver.ResolveName(typeof(DefaultScopedService), ServiceLifetime.Scoped);
        Assert.Equal("defaultScopedService", name);
    }

    [Fact]
    public void Should_Fallback_To_CamelCase_If_Name_Is_Not_Provided_Transient()
    {
        var name = NameResolver.ResolveName(typeof(DefaultTransientService), ServiceLifetime.Transient);
        Assert.Equal("defaultTransientService", name);
    }

    [Fact]
    public void Should_Default_To_CamelCase_When_Attribute_Not_Found()
    {
        var name = NameResolver.ResolveName(typeof(UnattributedClass), (ServiceLifetime)999); // 잘못된 enum
        Assert.Equal("unattributedClass", name);
    }
}

[Singleton("customSingleton")]
public class NamedSingletonService
{
}

[Scoped]
public class DefaultScopedService
{
}

[Transient]
public class DefaultTransientService
{
}

public class UnattributedClass
{
}