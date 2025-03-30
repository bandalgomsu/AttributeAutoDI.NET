using AttributeAutoDI.Attribute;

namespace Test.AttributeInjectionTest;

// ISampleService.cs
public interface ISampleService
{
    Guid GetId();
}

// SampleService.cs
[Singleton("sampleService")]
public class SampleService : ISampleService
{
    private readonly Guid _id = Guid.NewGuid();

    public Guid GetId()
    {
        return _id;
    }
}

// SamplePrimaryService.cs
[Singleton("samplePrimaryService")]
[Primary]
public class SamplePrimaryService : ISampleService
{
    private readonly Guid _id = Guid.NewGuid();

    public Guid GetId()
    {
        return _id;
    }
}