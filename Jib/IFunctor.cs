using System;

namespace Jib
{
    public interface IFunctor<out A>
    {
        IFunctor<B> Map<B>(Func<A, B> f);
        object Value { get; }
    }
}