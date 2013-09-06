namespace Jib
{
    public interface IMonoid<T> : ISemigroup<T>
    {
        T Zero { get; }
    }
}