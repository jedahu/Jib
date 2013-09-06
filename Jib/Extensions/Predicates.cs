using System;

namespace Jib.Extensions
{
    public static class MaybePredicates
    {
        public static bool JustTest<A>(this Maybe<A> maybe, Func<A, bool> predicate)
        {
            return maybe.Cata(predicate, () => false);
        }

        public static bool JustEq<A>(this Maybe<A> maybe, A other, IEq<A> aEq)
        {
            return maybe.JustTest(a => aEq.Eq(a, other));
        }
    }

    public static class EitherPredicates
    {
        public static bool LeftTest<A, X>(this Either<A, X> either, Func<A, bool> predicate)
        {
            return either.Cata(predicate, x => false);
        }

        public static bool RightTest<A, X>(this Either<A, X> either, Func<X, bool> predicate)
        {
            return either.Cata(a => false, predicate);
        }

        public static bool LeftEq<A, X>(this Either<A, X> either, A other, IEq<A> aEq)
        {
            return either.LeftTest(a => aEq.Eq(a, other));
        }

        public static bool RightEq<A, X>(this Either<A, X> either, X other, IEq<X> xEq)
        {
            return either.RightTest(x => xEq.Eq(x, other));
        }
    }
}
