using System;

namespace Jib.Extensions
{
    public static class MaybeLinq
    {
        public static Maybe<B> Select<A, B>(this Maybe<A> maybe, Func<A, B> selectFunc)
        {
            return maybe.Map(selectFunc);
        }

        public static Maybe<B> SelectMany<A, B>(this Maybe<A> maybe, Func<A, Maybe<B>> selectManyFunc)
        {
            return maybe.Bind(selectManyFunc);
        }

        public static Maybe<C> SelectMany<A, B, C>(this Maybe<A> maybe, Func<A, Maybe<B>> selectManyFunc, Func<A, B, C> combineFunc)
        {
            return maybe.Bind(a => selectManyFunc(a).Bind(b => Maybe.Just(combineFunc(a, b))));
        }

        public static Maybe<A> Where<A>(this Maybe<A> maybe, Func<A, bool> predicate)
        {
            return maybe.Cata(a => predicate(a) ? maybe : Maybe.Nothing<A>(), Maybe.Nothing<A>);
        }
    }

    public static class EitherLinq
    {
        public static Either<B, X> Select<A, B, X>(this Either<A, X> either, Func<A, B> selectFunc)
        {
            return either.Map(selectFunc);
        }

        public static Either<B, X> SelectMany<A, B, X>(this Either<A, X> either, Func<A, Either<B, X>> selectManyFunc)
        {
            return either.Bind(selectManyFunc);
        }

        public static Either<C, X> SelectMany<A, B, C, X>(this Either<A, X> either, Func<A, Either<B, X>> selectManyFunc, Func<A, B, C> combineFunc)
        {
            return either.Bind(a => selectManyFunc(a).Bind(b => Either.Left<C, X>(combineFunc(a, b))));
        }
    }

    public static class ValidationLinq
    {
        public static Validation<B, X> Select<A, B, X>(this Validation<A, X> either, Func<A, B> selectFunc)
        {
            return either.Map(selectFunc);
        }

        public static Validation<B, X> SelectMany<A, B, X>(this Validation<A, X> either, Func<A, Validation<B, X>> selectManyFunc)
        {
            return either.Bind(selectManyFunc);
        }

        public static Validation<C, X> SelectMany<A, B, C, X>(this Validation<A, X> either, Func<A, Validation<B, X>> selectManyFunc, Func<A, B, C> combineFunc)
        {
            return either.Bind(a => selectManyFunc(a).Bind(b => Validation.Success<C, X>(combineFunc(a, b))));
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
