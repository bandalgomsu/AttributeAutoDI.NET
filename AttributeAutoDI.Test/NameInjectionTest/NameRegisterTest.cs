using AttributeAutoDI.Internal.NameInjection;

namespace Test.NameInjectionTest;

[Collection("AttributeAutoDI.Test")]
public class NameRegisterTest
{
    private readonly Type _implType = typeof(MyNamedService);
    private readonly Type _serviceType = typeof(IMyService);

    public NameRegisterTest()
    {
        NameRegister.Clear(); // 항상 초기화
    }

    [Fact]
    public void Register_Should_Store_Entry()
    {
        NameRegister.Register(_serviceType, "myName", _implType);

        var map = NameRegister.GetMap();
        Assert.True(map.ContainsKey((_serviceType, "myName")));
        Assert.Equal(_implType, map[(_serviceType, "myName")]);
    }

    [Fact]
    public void Register_Should_Throw_On_Duplicate()
    {
        NameRegister.Register(_serviceType, "duplicate", _implType);

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            NameRegister.Register(_serviceType, "duplicate", typeof(AnotherService));
        });

        Assert.Contains("Duplicate", ex.Message);
    }

    [Fact]
    public void Resolve_Should_Return_ImplType()
    {
        NameRegister.Register(_serviceType, "resolveMe", _implType);

        var resolved = NameRegister.Resolve(_serviceType, "resolveMe");

        Assert.Equal(_implType, resolved);
    }

    [Fact]
    public void Resolve_Should_Throw_When_Not_Registered()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => { NameRegister.Resolve(_serviceType, "notExist"); });

        Assert.Contains("[AttributeAutoDI \u274c] No named registration for", ex.Message);
    }

    [Fact]
    public void Clear_Should_Empty_Map()
    {
        NameRegister.Register(_serviceType, "clearMe", _implType);
        NameRegister.Clear();

        var map = NameRegister.GetMap();
        Assert.Empty(map);
    }
}

public interface IMyService
{
}

public class MyNamedService : IMyService
{
}

public class AnotherService : IMyService
{
}