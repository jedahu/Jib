namespace Jib
{
    public interface ISemigroup<T>
    {
        T Op(T t1, T t2);
    }
}