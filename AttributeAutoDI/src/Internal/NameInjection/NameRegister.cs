using System.Collections.Concurrent;

namespace AttributeAutoDI.Internal.NameInjection;

public static class NameRegister
{
    private static readonly ConcurrentDictionary<(Type ServiceType, string Name), Type> NameMap = new();

    public static void Register(Type serviceType, string name, Type implType)
    {
        var key = (serviceType, name);

        if (!NameMap.TryAdd(key, implType))
            throw new InvalidOperationException(
                $"[AttributeAutoDI ❌] Duplicate named registration: {serviceType.Name} with name '{name}'");

        Console.WriteLine($"[AttributeAutoDI ✅] Named mapping: {name} → {serviceType.Name}");
    }

    public static Type Resolve(Type serviceType, string name)
    {
        var key = (serviceType, name);

        if (!NameMap.TryGetValue(key, out var implType))
            throw new InvalidOperationException(
                $"[AttributeAutoDI ❌] No named registration for '{serviceType.Name}' with name '{name}'");

        return implType;
    }

    public static IReadOnlyDictionary<(Type ServiceType, string Name), Type> GetMap()
    {
        return NameMap;
    }

    public static void Clear()
    {
        NameMap.Clear();
    }
}