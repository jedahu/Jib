using System;
using System.Collections.Generic;
using Jib.Extensions;

namespace Jib
{
    public static class LazyList
    {
        public static LazyList<A> Empty<A>()
        {
            return new LazyList<A>();
        }

        public static LazyList<A> Single<A>(A head)
        {
            return new LazyList<A>(head, Empty<A>);
        }

        public static LazyList<A> Create<A>(Tuple<A, IEnumerable<A>> cons)
        {
            return new LazyList<A>(cons.Item1, () => Create(cons.Item2));
        }

        public static LazyList<A> Create<A>(IEnumerable<A> enumerable)
        {
            return enumerable.Uncons().Map(Create).ValueOr(Empty<A>);
        }
    }
}