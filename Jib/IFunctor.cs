using System;

namespace Jib
{
    public interface IFunctor<out A>
    {
        IFunctor<B> Map<B>(Func<A, B> f);
        dynamic Value { get; }
    }
}