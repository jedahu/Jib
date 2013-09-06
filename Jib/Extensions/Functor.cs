using System;

namespace Jib.Extensions
{
    public static class MaybeFunctor
    {
        public static Maybe<B> Map<A, B>(this Maybe<A> maybe, Func<A, B> f)
        {
            return maybe.Cata(
                a => Maybe.Just(f(a)),
                Maybe.Nothing<B>);
        }
    }

    public static class EitherFunctor
    {
        public static Either<B, X> Map<A, B, X>(this Either<A, X> either, Func<A, B> f)
        {
            return either.Cata(a => Either.Left<B, X>(f(a)), Either.Right<B, X>);
        }

        public static Either<A, Y> MapRight<A, X, Y>(this Either<A, X> either, Func<X, Y> f)
        {
            return either.Swap(e => e.Map(f));
        }
    }

    public static class PromiseFunctor
    {
        public static Promise<B> Map<A, B>(this Promise<A> promise, Func<A, B> f)
        {
            var p = new Promise<B>();
            p.Strategy.Call(() => p.Signal(f(promise.Wait)));
            return p;
        }
    }

    public static class FutureFunctor
    {
        public static Future<B> Map<A, B>(this Future<A> future, Func<A, B> f)
        {
            return new Future<B>(
                cb => future.Callback(a => cb(f(a))),
                future.Strategy);
        }
    }
}