using System.Reflection;
using AttributeAutoDI.Attribute;
using AttributeAutoDI.Internal.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Test.ConfigurationTest;

[Collection("AttributeAutoDI.Test")]
public class ConfigurationExecutionTests
{
    private static readonly List<string> Log = new();

    [Fact]
    public void Should_Execute_Pre_Configuration_Method()
    {
        // Arrange
        Log.Clear();
        var services = new ServiceCollection();

        // Act
        services.UsePreConfiguration(Assembly.GetExecutingAssembly());

        // Assert
        Assert.Contains("PreInit", Log);
    }

    [Fact]
    public void Should_Execute_Post_Configuration_Method()
    {
        // Arrange
        Log.Clear();
        var services = new ServiceCollection();

        // Act
        services.UsePostConfiguration(Assembly.GetExecutingAssembly());

        // Assert
        Assert.Contains("PostCleanup", Log);
    }

    [PreConfiguration]
    public static class MyPreConfig
    {
        [Execute]
        public static void Init(IServiceCollection services)
        {
            Log.Add("PreInit");
        }
    }

    [PostConfiguration]
    public static class MyPostConfig
    {
        [Execute]
        public static void Cleanup(IServiceCollection services)
        {
            Log.Add("PostCleanup");
        }
    }
}