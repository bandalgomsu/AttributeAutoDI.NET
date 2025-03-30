namespace Test.PrimaryInjectionTest;

public interface IPrimaryService
{
    Guid Id { get; }
}

public class PrimaryServiceImpl : IPrimaryService
{
    public Guid Id { get; } = Guid.NewGuid();
}