using System;

namespace Jib.Syntax
{
    public static class MaybePredicates
    {
        public static bool JustTest<A>(this Maybe<A> maybe, Func<A, bool> predicate)
        {
            return maybe.Cata(() => false, predicate);
        }

        public static bool JustEq<A>(this Maybe<A> maybe, A other, IEq<A> aEq)
        {
            return maybe.JustTest(a => aEq.Eq(a, other));
        }
    }

    public static class EitherPredicates
    {
        public static bool RightTest<X, A>(this Either<X, A> either, Func<A, bool> predicate)
        {
            return either.Cata(x => false, predicate);
        }

        public static bool LeftTest<X, A>(this Either<X, A> either, Func<X, bool> predicate)
        {
            return either.Cata(predicate, a => false);
        }

        public static bool RightEq<X, A>(this Either<X, A> either, A other, IEq<A> aEq)
        {
            return either.RightTest(a => aEq.Eq(a, other));
        }

        public static bool LeftEq<X, A>(this Either<X, A> either, X other, IEq<X> xEq)
        {
            return either.LeftTest(x => xEq.Eq(x, other));
        }
    }
}
