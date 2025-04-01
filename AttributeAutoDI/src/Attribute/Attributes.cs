namespace AttributeAutoDI.Attribute;

[AttributeUsage(AttributeTargets.Class)]
public class SingletonAttribute : System.Attribute
{
    public SingletonAttribute(string? name = null)
    {
        Name = name;
    }

    public string? Name { get; }
}

[AttributeUsage(AttributeTargets.Class)]
public class ScopedAttribute : System.Attribute
{
    public ScopedAttribute(string? name = null)
    {
        Name = name;
    }

    public string? Name { get; }
}

[AttributeUsage(AttributeTargets.Class)]
public class TransientAttribute : System.Attribute
{
    public TransientAttribute(string? name = null)
    {
        Name = name;
    }

    public string? Name { get; }
}

[AttributeUsage(AttributeTargets.Class)]
public class PrimaryAttribute : System.Attribute
{
}

[AttributeUsage(AttributeTargets.Parameter)]
public class NamedAttribute : System.Attribute
{
    public NamedAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}

[AttributeUsage(AttributeTargets.Class)]
public class OptionsAttribute : System.Attribute
{
    public OptionsAttribute(string section)
    {
        Section = section;
    }

    public string Section { get; }
}

// //todo
// [AttributeUsage(AttributeTargets.Method)]
// public class ProfileAttribute(string env) : System.Attribute
// {
//     public string EnvironmentName { get; } = env;
// }
// //todo
// [AttributeUsage(AttributeTargets.Class)]
// public class ConfigurationAttribute : System.Attribute
// {
// }