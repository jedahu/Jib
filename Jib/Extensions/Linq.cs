using System;

namespace Jib.Extensions
{
    public static class MaybeLinq
    {
        public static Maybe<TB> Select<TA, TB>(this Maybe<TA> maybe, Func<TA, TB> selectFunc)
        {
            return maybe.Map(selectFunc);
        }

        public static Maybe<TB> SelectMany<TA, TB>(this Maybe<TA> maybe, Func<TA, Maybe<TB>> selectManyFunc)
        {
            return maybe.Bind(selectManyFunc);
        }

        public static Maybe<TC> SelectMany<TA, TB, TC>(this Maybe<TA> maybe, Func<TA, Maybe<TB>> selectManyFunc, Func<TA, TB, TC> combineFunc)
        {
            return maybe.Bind(a => selectManyFunc(a).Bind(b => Maybe.Just(combineFunc(a, b))));
        }

        public static Maybe<TA> Where<TA>(this Maybe<TA> maybe, Func<TA, bool> predicate)
        {
            return maybe.Cata(a => predicate(a) ? maybe : Maybe.Nothing<TA>(), Maybe.Nothing<TA>);
        }
    }

    public static class EitherLinq
    {
        public static Either<TB, TX> Select<TA, TB, TX>(this Either<TA, TX> either, Func<TA, TB> selectFunc)
        {
            return either.Map(selectFunc);
        }

        public static Either<TB, TX> SelectMany<TA, TB, TX>(this Either<TA, TX> either, Func<TA, Either<TB, TX>> selectManyFunc)
        {
            return either.Bind(selectManyFunc);
        }

        public static Either<TC, TX> SelectMany<TA, TB, TC, TX>(this Either<TA, TX> either, Func<TA, Either<TB, TX>> selectManyFunc, Func<TA, TB, TC> combineFunc)
        {
            return either.Bind(a => selectManyFunc(a).Bind(b => Either.Left<TC, TX>(combineFunc(a, b))));
        }
    }

    public static class PromiseLinq
    {
        
    }

    public static class FutureLinq
    {
        public static Future<B> Select<A, B>(this Future<A> m, Func<A, B> f)
        {
            return m.Map(f);
        }

        public static Future<B> SelectMany<A, B>(this Future<A> m, Func<A, Future<B>> k)
        {
            return m.Bind(k);
        }

        public static Future<C> SelectMany<A, B, C>(this Future<A> m, Func<A, Future<B>> k, Func<A, B, C> f)
        {
            return m.Bind(a => k(a).Bind<B, C>(b => f(a, b).PureFuture()));
        }
    }
}
