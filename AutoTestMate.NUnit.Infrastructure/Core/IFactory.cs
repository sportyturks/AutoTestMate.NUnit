namespace AutoTestMate.NUnit.Infrastructure.Core
{
    public interface IFactory<T>
    {
        T Create();
    }
}