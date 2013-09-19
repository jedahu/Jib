using System;
using System.Collections.Generic;
using Jib.Extensions;

namespace Jib
{
    public static class NonEmptyLazyList
    {
        public static NonEmptyLazyList<A> Single<A>(A head)
        {
            return new NonEmptyLazyList<A>(head, Maybe.Nothing<NonEmptyLazyList<A>>);
        }

        public static NonEmptyLazyList<A> Create<A>(Pair<A, IEnumerable<A>> cons)
        {
            return cons.Fst.Cons(cons.Snd);
        }

        public static NonEmptyLazyList<A> Cons<A>(this A head, NonEmptyLazyList<A> nel)
        {
            return new NonEmptyLazyList<A>(head, nel.PureMaybe);
        }

        public static NonEmptyLazyList<A> Cons<A>(this A head, IEnumerable<A> tail)
        {
            return new NonEmptyLazyList<A>(
                head,
                () => tail.Uncons().Map(Create));
        }
    }
}