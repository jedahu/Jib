using System;
using System.Collections;
using System.Collections.Generic;

namespace Jib
{
    public sealed class EmptyLazyList<T> : ILazyList<T>, IUncons<T>
    {
        public Maybe<Tuple<T, ILazyList<T>>> Uncons
        {
            get { return Maybe.Nothing<Tuple<T, ILazyList<T>>>(); }
        }

        public IEnumerator<T> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}