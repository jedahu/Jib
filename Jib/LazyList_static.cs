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

        public static LazyList<A> Create<A>(Pair<A, IEnumerable<A>> cons)
        {
            return new LazyList<A>(cons.Fst, () => Create(cons.Snd));
        }

        public static LazyList<A> Create<A>(IEnumerable<A> enumerable)
        {
            return enumerable.Uncons().Map(Create).ValueOr(Empty<A>);
        }
    }
}