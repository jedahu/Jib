namespace Jib
{
    public interface IEq<in A>
    {
        bool Eq(A t1, A t2);
    }
}
