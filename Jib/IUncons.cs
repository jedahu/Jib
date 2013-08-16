using System;

namespace Jib
{
    internal interface IUncons<T>
    {
        Maybe<Tuple<T, ILazyList<T>>> Uncons { get; }
    }
}