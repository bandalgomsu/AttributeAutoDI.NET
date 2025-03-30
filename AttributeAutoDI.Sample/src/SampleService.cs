using AttributeAutoDI.Attribute;

namespace AttributeAutoDI.Sample;

public interface ISingletonService
{
    string GetMessage();
}

[Primary]
[Singleton]
public class SampleService : ISingletonService
{
    public string GetMessage()
    {
        return "PrimarySingleton";
    }
}

[Singleton("NamedDI")]
public class SampleNamedDiService : ISingletonService
{
    public string GetMessage()
    {
        return "NamedSingleton";
    }
}

public interface IScopedService
{
    string GetMessage();
}

[Scoped]
public class ScopedService : IScopedService
{
    public string GetMessage()
    {
        return "Scoped";
    }
}

[Transient]
public class TransientService
{
    public string GetMessage()
    {
        return "Transient";
    }
}