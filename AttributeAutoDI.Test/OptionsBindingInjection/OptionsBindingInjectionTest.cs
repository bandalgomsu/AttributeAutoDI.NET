using AttributeAutoDI.Attribute;
using AttributeAutoDI.Internal.OptionsBindingInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Test.OptionsBindingInjection;

[Collection("AttributeAutoDI.Test")]
public class OptionsBindingInjectionTests
{
    [Fact]
    public void Should_Bind_Options_From_Configuration()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>
        {
            { "App:Name", "MyApp" },
            { "App:Timeout", "30" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var services = new ServiceCollection();

        // Act
        services.UseOptionsBindingInjection(configuration, typeof(AppSettings.AppOptions).Assembly);

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<AppSettings.AppOptions>>().Value;

        // Assert
        Assert.Equal("MyApp", options.Name);
        Assert.Equal(30, options.Timeout);
    }

    public class AppSettings
    {
        [Options("App")]
        public class AppOptions
        {
            public string Name { get; set; }
            public int Timeout { get; set; }
        }
    }
}