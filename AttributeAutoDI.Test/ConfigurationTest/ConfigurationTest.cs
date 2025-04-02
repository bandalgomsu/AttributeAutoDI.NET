using AttributeAutoDI.Attribute;
using AttributeAutoDI.Internal.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Test.ConfigurationTest;

[Collection("AttributeAutoDI.Test")]
public class ConfigurationExecutionTests
{
    public static List<string> Logs = new();

    [Fact]
    public void Should_Execute_Pre_And_Post_Configuration_Methods()
    {
        // Arrange
        Logs.Clear();
        var services = new ServiceCollection();
        var configData = new Dictionary<string, string> { { "TestKey", "TestValue" } };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act
        services.UsePreConfiguration(configuration, typeof(TestPreConfig).Assembly);
        services.UsePostConfiguration(configuration, typeof(TestPreConfig).Assembly);

        // Assert
        Assert.Contains("PRE", Logs);
        Assert.Contains("PRE1", Logs);
        Assert.Contains("PRE2", Logs);
        Assert.Contains("PRE3", Logs);

        Assert.Contains("POST", Logs);
        Assert.Contains("POST1", Logs);
        Assert.Contains("POST2", Logs);
        Assert.Contains("POST3", Logs);
    }
}

[PreConfiguration]
public static class TestPreConfig
{
    [Execute]
    public static void Run(IServiceCollection services, IConfiguration config)
    {
        ConfigurationExecutionTests.Logs.Add("PRE");
    }

    [Execute]
    public static void Run1(IServiceCollection services)
    {
        ConfigurationExecutionTests.Logs.Add("PRE1");
    }

    [Execute]
    public static void Run2(this IServiceCollection services)
    {
        ConfigurationExecutionTests.Logs.Add("PRE2");
    }

    [Execute]
    public static void Run3(this IServiceCollection services, IConfiguration config)
    {
        ConfigurationExecutionTests.Logs.Add("PRE3");
    }
}

[PostConfiguration]
public static class TestPostConfig
{
    [Execute]
    public static void Run(IServiceCollection services, IConfiguration config)
    {
        ConfigurationExecutionTests.Logs.Add("POST");
    }

    [Execute]
    public static void Run1(IServiceCollection services)
    {
        ConfigurationExecutionTests.Logs.Add("POST1");
    }

    [Execute]
    public static void Run2(this IServiceCollection services)
    {
        ConfigurationExecutionTests.Logs.Add("POST2");
    }

    [Execute]
    public static void Run3(this IServiceCollection services, IConfiguration config)
    {
        ConfigurationExecutionTests.Logs.Add("POST3");
    }
}