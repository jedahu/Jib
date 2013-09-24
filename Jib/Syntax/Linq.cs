using System;

namespace Jib.Syntax
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
            return maybe.Cata(Maybe.Nothing<A>, a => predicate(a) ? maybe : Maybe.Nothing<A>());
        }
    }

    public static class EitherLinq
    {
        public static Either<X, B> Select<X, A, B>(this Either<X, A> either, Func<A, B> selectFunc)
        {
            return either.Map(selectFunc);
        }

        public static Either<X, B> SelectMany<X, A, B>(this Either<X, A> either, Func<A, Either<X, B>> selectManyFunc)
        {
            return either.Bind(selectManyFunc);
        }

        public static Either<X, C> SelectMany<X, A, B, C>(this Either<X, A> either, Func<A, Either<X, B>> selectManyFunc, Func<A, B, C> combineFunc)
        {
            return either.Bind(a => selectManyFunc(a).Bind(b => Either.Right<X, C>(combineFunc(a, b))));
        }
    }

    public static class ValidationLinq
    {
        public static Validation<X, B> Select<X, A, B>(this Validation<X, A> either, Func<A, B> selectFunc)
        {
            return either.Map(selectFunc);
        }

        public static Validation<X, B> SelectMany<X, A, B>(this Validation<X, A> either, Func<A, Validation<X, B>> selectManyFunc)
        {
            return either.Bind(selectManyFunc);
        }

        public static Validation<X, C> SelectMany<X, A, B, C>(this Validation<X, A> either, Func<A, Validation<X, B>> selectManyFunc, Func<A, B, C> combineFunc)
        {
            return either.Bind(a => selectManyFunc(a).Bind(b => Validation.Success<X, C>(combineFunc(a, b))));
        }
    }

    public static class PromiseLinq
    {
        public static Promise<B> Select<A, B>(this Promise<A> m, Func<A, B> f)
        {
            return m.Map(f);
        }

        public static Promise<B> SelectMany<A, B>(this Promise<A> m, Func<A, Promise<B>> k)
        {
            return m.Bind(k);
        }

        public static Promise<C> SelectMany<A, B, C>(this Promise<A> m, Func<A, Promise<B>> k, Func<A, B, C> f)
        {
            return m.Bind(a => k(a).Bind<B, C>(b => f(a, b).PurePromise()));
        }
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

    public static class NonEmptyLazyListLinq
    {
        public static NonEmptyLazyList<B> Select<A, B>(this NonEmptyLazyList<A> m, Func<A, B> f)
        {
            return m.Map(f);
        }

        public static NonEmptyLazyList<B> SelectMany<A, B>(this NonEmptyLazyList<A> m, Func<A, NonEmptyLazyList<B>> k)
        {
            return m.Bind(k);
        }

        public static NonEmptyLazyList<C> SelectMany<A, B, C>(this NonEmptyLazyList<A> m, Func<A, NonEmptyLazyList<B>> k, Func<A, B, C> f)
        {
            return m.Bind(a => k(a).Bind<B, C>(b => f(a, b).PureNonEmptyLazyList()));
        }
    }
}
