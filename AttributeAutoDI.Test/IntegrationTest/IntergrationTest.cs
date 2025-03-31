using AttributeAutoDI.Attribute;
using AttributeAutoDI.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Test.IntegrationTest;

[Collection("AttributeAutoDI.Test")]
public class IntegrationTest
{
    [Fact]
    public async Task Should_Use_Named_Implementation_In_Controller()
    {
        // Arrange: 직접 앱 호스팅
        using var host = await new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices((context, services) =>
                    {
                        var configuration = context.Configuration;
                        services.AddControllers();
                        services.AddAttributeDependencyInjection(configuration, typeof(IntegrationTest).Assembly);
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
                    });
            })
            .StartAsync();

        var client = host.GetTestClient();

        // Act
        var response = await client.GetAsync("/notify");
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal("Email Sent!", body);
    }
}

public interface INotificationService
{
    string Notify();
}

[Singleton("email")]
public class EmailNotificationService : INotificationService
{
    public string Notify()
    {
        return "Email Sent!";
    }
}

[Singleton]
[Primary]
public class SmsNotificationService : INotificationService
{
    public string Notify()
    {
        return "SMS Sent!";
    }
}

public class NotifyController([Named("email")] INotificationService service) : ControllerBase
{
    [HttpGet("/notify")]
    public string Notify()
    {
        return service.Notify();
    }
}