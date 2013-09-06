using System;

namespace Jib.Extensions
{
    public static class MaybeFoldable
    {
        public static R FoldRight<A, R>(this Maybe<A> maybe, Func<A, R, R> f, R seed)
        {
            return maybe.Cata(a => f(a, seed), () => seed);
        }

        public static R FoldLeft<A, R>(this Maybe<A> maybe, Func<R, A, R> f, R seed)
        {
            return maybe.Cata(r => f(seed, r), () => seed);
        }

        public static T Reduce<T>(this Maybe<T> maybe, IMonoid<T> monoid)
        {
            return maybe.ValueOr(() => monoid.Zero);
        }
    }

    public static class EitherFoldable
    {
        public static R FoldRight<A, X, R>(this Either<A, X> either, Func<A, R, R> f, R seed)
        {
            return either.Cata(a => f(a, seed), x => seed);
        }

        public static R FoldLeft<A, X, R>(this Either<A, X> either, Func<R, A, R> f, R seed)
        {
            return either.Cata(a => f(seed, a), x => seed);
        }

        public static T Reduce<T, X>(this Either<T, X> either, IMonoid<T> monoid)
        {
            return either.LeftOr(() => monoid.Zero);
        }
    }
}
