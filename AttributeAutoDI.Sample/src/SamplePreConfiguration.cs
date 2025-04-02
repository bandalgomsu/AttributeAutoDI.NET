using AttributeAutoDI.Attribute;

namespace AttributeAutoDI.Sample;

[PreConfiguration]
public static class SamplePreConfiguration
{
    [Execute]
    public static void AddControllers(IServiceCollection services)
    {
        services.AddControllers();
        services.AddControllersWithViews();
    }
}