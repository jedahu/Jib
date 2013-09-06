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
                () => new[] {Maybe.Nothing<B>()});
        }

        public static IEnumerable<Maybe<A>> SequenceEnumerable<A>(this Maybe<IEnumerable<A>> maybe)
        {
            return maybe.Cata(
                a => a.Select(Maybe.Just),
                () => new[] {Maybe.Nothing<A>()});
        }
    }

    public static class EitherTraversable
    {
        public static IEnumerable<Either<B, X>> TraverseEnumerable<A, B, X>(this Either<A, X> either, Func<A, IEnumerable<B>> f)
        {
            return either.Cata(
                a => f(a).Select(Either.Left<B, X>),
                x => new[] {Either.Right<B, X>(x)});
        }

        public static IEnumerable<Either<A, X>> SequenceEnumerable<A, X>(this Either<IEnumerable<A>, X> either)
        {
            return either.Cata(
                a => a.Select(Either.Left<A, X>),
                x => new[] {Either.Right<A, X>(x)});
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
    }
}
