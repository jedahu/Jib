using System;
using System.Collections.Generic;
using System.Linq;

namespace Jib.Extensions
{
    public static class MaybeTraversable
    {
        public static IEnumerable<Maybe<B>> TraverseEnumerable<A, B>(this Maybe<A> maybe, Func<A, IEnumerable<B>> f)
        {
            return maybe.Cata(
                a => f(a).Select(Maybe.Just),
                () => Maybe.Nothing<B>().PureEnumerable());
        }

        public static IEnumerable<Maybe<A>> SequenceEnumerable<A>(this Maybe<IEnumerable<A>> maybe)
        {
            return maybe.Cata(
                a => a.Select(Maybe.Just),
                () => Maybe.Nothing<A>().PureEnumerable());
        }

        public static Future<Maybe<B>> TraverseFuture<A, B>(this Maybe<A> maybe, Func<A, Future<B>> f)
        {
            return maybe.Map(f).SequenceFuture();
        }

        public static Future<Maybe<A>> SequenceFuture<A>(this Maybe<Future<A>> maybe)
        {
            return maybe.Cata(
                a => a.Map(Maybe.Just),
                () => Maybe.Nothing<A>().PureFuture());
        }
    }

    public static class EitherTraversable
    {
        public static IEnumerable<Either<B, X>> TraverseEnumerable<A, B, X>(this Either<A, X> either, Func<A, IEnumerable<B>> f)
        {
            return either.Cata(
                a => f(a).Select(Either.Left<B, X>),
                x => Either.Right<B, X>(x).PureEnumerable());
        }

        public static IEnumerable<Either<A, X>> SequenceEnumerable<A, X>(this Either<IEnumerable<A>, X> either)
        {
            return either.Cata(
                a => a.Select(Either.Left<A, X>),
                x => Either.Right<A, X>(x).PureEnumerable());
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
