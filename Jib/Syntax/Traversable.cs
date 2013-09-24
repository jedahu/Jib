using System;
using System.Collections.Generic;
using System.Linq;

namespace Jib.Syntax
{
    public static class MaybeTraversable
    {
        public static IEnumerable<Maybe<B>> TraverseEnumerable<A, B>(this Maybe<A> maybe, Func<A, IEnumerable<B>> f)
        {
            return maybe.Cata(() => Maybe.Nothing<B>().PureEnumerable(), a => f(a).Select(Maybe.Just));
        }

        public static IEnumerable<Maybe<A>> SequenceEnumerable<A>(this Maybe<IEnumerable<A>> maybe)
        {
            return maybe.Cata(() => Maybe.Nothing<A>().PureEnumerable(), a => a.Select(Maybe.Just));
        }

        public static Future<Maybe<B>> TraverseFuture<A, B>(this Maybe<A> maybe, Func<A, Future<B>> f)
        {
            return maybe.Map(f).SequenceFuture();
        }

        public static Future<Maybe<A>> SequenceFuture<A>(this Maybe<Future<A>> maybe)
        {
            return maybe.Cata(() => Maybe.Nothing<A>().PureFuture(), a => a.Map(Maybe.Just));
        }
    }

    public static class EitherTraversable
    {
        public static IEnumerable<Either<X, B>> TraverseEnumerable<A, B, X>(this Either<X, A> either, Func<A, IEnumerable<B>> f)
        {
            return either.Cata(
                x => Either.Left<X, B>(x).PureEnumerable(),
                a => f(a).Select(Either.Right<X, B>));
        }

        public static IEnumerable<Either<X, A>> SequenceEnumerable<X, A>(this Either<X, IEnumerable<A>> either)
        {
            return either.Cata(
                x => Either.Left<X, A>(x).PureEnumerable(),
                a => a.Select(Either.Right<X, A>));
        }
    }

    public static class PromiseTraversable
    {
        public static IEnumerable<Promise<B>> TraverseEnumerable<A, B>(this Promise<A> promise, Func<A, IEnumerable<B>> f)
        {
            return f(promise.Wait).Select(Promise.Now);
        }

        public static IEnumerable<Promise<T>> SequenceEnumerable<T>(this Promise<IEnumerable<T>> promise)
        {
            return promise.Wait.Select(Promise.Now);
        }
    }

    // Implementable without running the future?
    public static class FutureTraversable
    {
        public static IEnumerable<Future<B>> TraverseEnumerable<A, B>(this Future<A> future, Func<A, IEnumerable<B>> f)
        {
            return future.Map(f).SequenceEnumerable();
        }

        public static IEnumerable<Future<T>> SequenceEnumerable<T>(this Future<IEnumerable<T>> future)
        {
            return future.Map(ts => ts.Select(Future.Now)).Run();
        }

        public static Maybe<Future<B>> TraverseMabye<A, B>(this Future<A> future, Func<A, Maybe<B>> f)
        {
            return future.Map(f).SequenceMaybe();
        }

        public static Maybe<Future<A>> SequenceMaybe<A>(this Future<Maybe<A>> future)
        {
            return future.Map(m => m.Map(Future.Now)).Run();
        }
    }
}
