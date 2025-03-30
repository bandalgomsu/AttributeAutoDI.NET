using AttributeAutoDI.Internal.AttributeInjection;
using AttributeAutoDI.Internal.NameInjection;
using AttributeAutoDI.Internal.PrimaryInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Test.AttributeInjectionTest;

[Collection("AttributeAutoDI.Test")]
public class AttributeInjectionTests
{
    [Fact]
    public void Should_Register_Singleton_Service()
    {
        NameRegister.Clear();
        PrimaryInjection.Clear();

        var services = new ServiceCollection();

        services.UseAttributeInjection(typeof(ISampleService).Assembly);

        var provider = services.BuildServiceProvider();

        var a = provider.GetRequiredService<ISampleService>();
        var b = provider.GetRequiredService<ISampleService>();

        Assert.Equal(a.GetId(), b.GetId()); // Singleton → 같은 인스턴스

        var implType = NameRegister.Resolve(typeof(ISampleService), "sampleService");
        Assert.Equal(typeof(SampleService), implType);
    }

    [Fact]
    public void Should_Register_Primary_Only_In_Map()
    {
        NameRegister.Clear();
        PrimaryInjection.Clear();
        var services = new ServiceCollection();

        services.UseAttributeInjection(typeof(ISampleService).Assembly);

        var primaryMap = PrimaryInjection.GetMap();
        var key = typeof(ISampleService);

        Assert.True(primaryMap.ContainsKey(key));
        Assert.Equal(typeof(SamplePrimaryService), primaryMap[key].implType);
    }
}