namespace AttributeAutoDI.Attribute;

[AttributeUsage(AttributeTargets.Class)]
public class SingletonAttribute(string? name = null) : System.Attribute
{
    public string? Name { get; } = name;
}

[AttributeUsage(AttributeTargets.Class)]
public class ScopedAttribute(string? name = null) : System.Attribute
{
    public string? Name { get; } = name;
}

[AttributeUsage(AttributeTargets.Class)]
public class TransientAttribute(string? name = null) : System.Attribute
{
    public string? Name { get; } = name;
}

[AttributeUsage(AttributeTargets.Class)]
public class PrimaryAttribute : System.Attribute
{
}

[AttributeUsage(AttributeTargets.Parameter)]
public class NamedAttribute(string name) : System.Attribute
{
    public string Name { get; } = name;
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