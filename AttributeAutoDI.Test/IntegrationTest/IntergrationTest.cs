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
    public static List<string> Logs = new();

    [Fact]
    public async Task Integration_Test()
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
        var namedResponse = await client.GetAsync("/named");
        var namedBody = await namedResponse.Content.ReadAsStringAsync();

        var primaryResponse = await client.GetAsync("/primary");
        var primaryBody = await primaryResponse.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal("Email Sent!", namedBody);
        Assert.Equal("SMS Sent!", primaryBody);

        Assert.Equal("PRE_CONFIG", Logs[0]);
        Assert.Equal("POST_CONFIG", Logs[1]);
    }
}

[PreConfiguration]
public static class PreConfiguration
{
    [Execute]
    public static void TestConfig(this IServiceCollection service)
    {
        IntegrationTest.Logs.Add("PRE_CONFIG");
        service.AddControllers();
    }
}

[PostConfiguration]
public static class PostConfiguration
{
    [Execute]
    public static void TestConfig(IServiceCollection service)
    {
        IntegrationTest.Logs.Add("POST_CONFIG");
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

public class NotifyController(
    [Named("email")] INotificationService namedNotificationService,
    INotificationService primarySmsNotificationService
) : ControllerBase
{
    [HttpGet("/named")]
    public string Named()
    {
        return namedNotificationService.Notify();
    }

    [HttpGet("/primary")]
    public string Primary()
    {
        return primarySmsNotificationService.Notify();
    }
}