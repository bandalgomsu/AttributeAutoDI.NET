using AttributeAutoDI.Internal.NameInjection;
using AttributeAutoDI.Internal.PrimaryInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Test.PrimaryInjectionTest;

[Collection("AttributeAutoDI.Test")]
public class PrimaryInjectionTest
{
    public PrimaryInjectionTest()
    {
        // 항상 초기화!
        PrimaryInjection.Clear();
        NameRegister.Clear();
    }

    [Fact]
    public void Add_Should_Store_Primary_Correctly()
    {
        // Arrange
        var serviceType = typeof(IPrimaryService);
        var implType = typeof(PrimaryServiceImpl);

        // Act
        PrimaryInjection.Add(serviceType, ServiceLifetime.Singleton, "testPrimary", implType);

        // Assert
        var map = PrimaryInjection.GetMap();
        Assert.True(map.ContainsKey(serviceType));
        Assert.Equal(implType, map[serviceType].implType);
        Assert.Equal("testPrimary", map[serviceType].Name);
    }

    [Fact]
    public void Add_Should_Throw_On_Duplicate()
    {
        // Arrange
        var serviceType = typeof(IPrimaryService);
        var implType = typeof(PrimaryServiceImpl);

        PrimaryInjection.Add(serviceType, ServiceLifetime.Singleton, "testPrimary", implType);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
        {
            PrimaryInjection.Add(serviceType, ServiceLifetime.Singleton, "another", implType);
        });
    }

    [Fact]
    public void UsePrimaryInjection_Should_Register_To_ServiceCollection()
    {
        // Arrange
        var serviceType = typeof(IPrimaryService);
        var implType = typeof(PrimaryServiceImpl);

        var services = new ServiceCollection();
        PrimaryInjection.Add(serviceType, ServiceLifetime.Singleton, "testPrimary", implType);

        // Act
        services.UsePrimaryInjection();

        var provider = services.BuildServiceProvider();
        var resolved = provider.GetRequiredService<IPrimaryService>();

        // Assert
        Assert.Equal(implType, resolved.GetType());

        // Also verify NameRegister contains it
        var named = NameRegister.Resolve(serviceType, "testPrimary");
        Assert.Equal(implType, named);

        // Map should be cleared
        Assert.Empty(PrimaryInjection.GetMap());
    }

    [Fact]
    public void Clear_Should_Empty_Map()
    {
        // Arrange
        PrimaryInjection.Add(typeof(IPrimaryService), ServiceLifetime.Singleton, "primary", typeof(PrimaryServiceImpl));

        // Act
        PrimaryInjection.Clear();

        // Assert
        Assert.Empty(PrimaryInjection.GetMap());
    }
}