using System;

namespace Jib
{
    public interface IStrategy
    {
        Func<A> Call<A>(Func<A> f);
        void Call(Action g);
    }
}